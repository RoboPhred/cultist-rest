using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ceen;
using CSRestAPI;
using CSRestAPI.JsonTranslation;
using CSRestAPI.Payloads;
using CSRestAPI.Server;
using CSRestAPI.Server.Attributes;
using CSRestAPI.Server.Exceptions;
using UnityEngine;

/// <summary>
/// This is the entry point for the CSRestAPI mod.
/// </summary>
public class CSRest : MonoBehaviour
{
    /// <summary>
    /// The web router instance.
    /// </summary>
    private readonly WebRouter router = new();

    /// <summary>
    /// The web server instance.
    /// </summary>
    private WebServer webServer;

    /// <summary>
    /// Initialize the mod.
    /// </summary>
    public static void Initialise()
    {
        try
        {
            new GameObject().AddComponent<CSRest>();
        }
        catch (Exception ex)
        {
            Logging.LogError($"Failed to initialize CSRest: {ex}");
        }
    }

    /// <summary>
    /// Unity callback for when the game object is created.
    /// This registers our controllers and starts the server.
    /// </summary>
    public void Start()
    {
        try
        {
            Dispatcher.Initialize();

            var ownAssembly = typeof(CSRest).Assembly;
            JsonTranslator.LoadJsonTranslatorStrategies(ownAssembly);
            this.RegisterControllers(ownAssembly);
        }
        catch (Exception ex)
        {
            Logging.LogError($"Failed to start CSRest: {ex}");
            return;
        }

        try
        {
            this.StartServer();
        }
        catch (Exception ex)
        {
            Logging.LogError($"Failed to start CSRest: {ex}");
        }
    }

    private void RegisterControllers(Assembly assembly)
    {
        var controllerTypes = (from type in assembly.GetTypes()
                               where type.GetCustomAttribute(typeof(WebControllerAttribute)) != null
                               select type).ToArray();

        var controllerRoutes = (from type in controllerTypes
                                let controller = Activator.CreateInstance(type)
                                let routes = WebControllerFactory.CreateRoutesFromController(controller)
                                from route in routes
                                select route).ToArray();

        foreach (var route in controllerRoutes)
        {
            this.router.AddRoute(route);
        }

        Logging.LogTrace($"Loaded {controllerRoutes.Length} routes from {controllerTypes.Length} controllers in {assembly.FullName}");
    }

    private void StartServer()
    {
        if (this.webServer != null)
        {
            return;
        }

        this.webServer = new WebServer(this.OnRequest);
        this.webServer.Start(8081);
    }

    private async Task<bool> OnRequest(IHttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            // For a proper implementation of CORS, see https://github.com/expressjs/cors/blob/master/lib/index.js#L159
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");

            // TODO: Choose based on available routes at this path
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, DELETE");
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
            context.Response.AddHeader("Access-Control-Max-Age", "1728000");
            context.Response.AddHeader("Access-Control-Expose-Headers", "Authorization");
            context.Response.StatusCode = HttpStatusCode.NoContent;
            context.Response.Headers["Content-Length"] = "0";
            return true;
        }
        else
        {
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Expose-Headers", "Authorization");
        }

        try
        {
            return await this.router.HandleRequest(context);
        }
        catch (WebException e)
        {
            await context.SendResponse(e.StatusCode, new ErrorPayload
            {
                Message = e.Message,
            });
            return true;
        }
    }
}
