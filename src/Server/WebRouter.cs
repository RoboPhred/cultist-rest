namespace CSRestAPI.Server
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ceen;

    /// <summary>
    /// A web router that routes requests in the form of <see href="IHttpContext"> objects to <see cref="IWebRoute"/> objects.
    /// </summary>
    public class WebRouter
    {
        private List<IWebRoute> routes = new List<IWebRoute>();

        /// <summary>
        /// Gets the number of routes in this router.
        /// </summary>
        public int Count
        {
            get
            {
                return this.routes.Count;
            }
        }

        /// <summary>
        /// Adds a route to this router.
        /// </summary>
        /// <param name="route">The route to add.</param>
        public void AddRoute(IWebRoute route)
        {
            this.routes.Add(route);
        }

        /// <summary>
        /// Routes a request to a route in this router.
        /// </summary>
        /// <param name="context">The context to route.</param>
        /// <returns>A task for the web request.  Resolves to true if the request was handled, or false if it was not.</returns>
        public async Task<bool> HandleRequest(IHttpContext context)
        {
            var request = context.Request;

            foreach (var route in this.routes)
            {
                if (request.Method != route.Method)
                {
                    continue;
                }

                var pathParameters = this.MatchRoute(request.Path, route.Path);
                if (pathParameters == null)
                {
                    continue;
                }

                var routeContext = new WebRouteContext(context, pathParameters);
                await route.OnRequested(routeContext);
                return true;
            }

            return false;
        }

        private IDictionary<string, string> MatchRoute(string requestPath, string routePath)
        {
            var requestSegments = requestPath.Split('/');
            var routeSegments = routePath.Split('/');

            if (requestSegments.Length - 1 != routeSegments.Length)
            {
                return null;
            }

            var pathParams = new Dictionary<string, string>();

            for (var i = 0; i < routeSegments.Length; i++)
            {
                var pathSegment = requestSegments[i + 1];
                var matchSegment = routeSegments[i];

                if (matchSegment.StartsWith(":"))
                {
                    pathParams.Add(matchSegment.Substring(1), pathSegment);
                }
                else if (pathSegment.ToLower() != matchSegment.ToLower())
                {
                    return null;
                }
            }

            return pathParams;
        }
    }
}
