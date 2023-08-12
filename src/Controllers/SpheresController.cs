namespace CSRestAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Ceen;
    using CSRestAPI.JsonTranslation;
    using CSRestAPI.Server.Attributes;
    using CSRestAPI.Server.Exceptions;
    using Newtonsoft.Json.Linq;
    using SecretHistories.Entities;
    using SecretHistories.UI;

    /// <summary>
    /// A controller dealing with Spheres.
    /// </summary>
    [WebController(Path = "api/spheres")]
    internal class SpheresController
    {
        /// <summary>
        /// Handles GET requests for spheres.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "GET")]
        public async Task GetSpheres(IHttpContext context)
        {
            var items = await Dispatcher.RunOnMainThread(() =>
                (from sphere in Watchman.Get<HornedAxe>().GetSpheres()
                 let json = JsonTranslator.ObjectToJson(sphere)
                 select json).ToArray());

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        /// <summary>
        /// Handles GET requests for tokens of a sphere.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="sphereId">The ID of the sphere to get tokens for.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        [WebRouteMethod(Method = "GET", Path = ":sphereId/tokens")]
        public async Task GetSphereContents(IHttpContext context, string sphereId)
        {
            var items = await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSpheres().FirstOrDefault(x => x.Id == sphereId);
                if (!sphere)
                {
                    throw new NotFoundException("No sphere with the given ID exists.");
                }

                return from token in sphere.GetTokens()
                       where !token.Defunct
                       let json = this.TokenToJObject(token)
                       select json;
            });

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        private JObject TokenToJObject(Token token)
        {
            var json = new JObject();

            JsonTranslator.ObjectToJson(token, json);

            if (JsonTranslator.HasStrategyFor(token.Payload))
            {
                JsonTranslator.ObjectToJson(token.Payload, json);
            }

            return json;
        }
    }
}
