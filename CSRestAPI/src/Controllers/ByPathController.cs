namespace CSRestAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ceen;
    using CSRestAPI.JsonTranslation;
    using CSRestAPI.Payloads;
    using CSRestAPI.Server.Attributes;
    using CSRestAPI.Server.Exceptions;
    using Newtonsoft.Json.Linq;
    using SecretHistories.Entities;
    using SecretHistories.Fucine;
    using SecretHistories.Spheres;
    using SecretHistories.UI;

    /// <summary>
    /// A controller dealing with Spheres.
    /// </summary>
    [WebController(Path = "api/by-path")]
    internal class ByPathController
    {
        /// <summary>
        /// Gets all spheres at the root.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "GET", Path = "~/spheres")]
        public async Task GetSpheresAtRoot(IHttpContext context)
        {
            var items = await Dispatcher.RunOnMainThread(() =>
                (from sphere in FucineRoot.Get().GetSpheres()
                 let json = JsonTranslator.ObjectToJson(sphere)
                 select json).ToArray());

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        /// <summary>
        /// Gets a token from a path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path to get the item at.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "GET", Path = "**path")]
        public async Task GetItemAtPath(IHttpContext context, string path)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                return new FucinePath(path).WithItemAtAbsolutePath(
                    token => this.TokenToJObject(token),
                    sphere => JsonTranslator.ObjectToJson(sphere));
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Modifies an item from a path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the item.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "PUT", Path = "**path")]
        public async Task UpdateItemAtPath(IHttpContext context, string path)
        {
            var body = context.ParseBody<JObject>();

            var result = await Dispatcher.RunOnMainThread(() =>
            {
                return new FucinePath(path).WithItemAtAbsolutePath(
                    token =>
                    {
                        this.UpdateToken(body, token);
                        return this.TokenToJObject(token);
                    },
                    sphere => throw new BadRequestException("Cannot update a sphere."));
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Deletes the item at the specified path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the item to delete.</param>
        /// <returns>A task that resolves once the requst is completed.</returns>
        [WebRouteMethod(Method = "DELETE", Path = "**path")]
        public async Task DeleteItemAtPath(IHttpContext context, string path)
        {
            await Dispatcher.RunOnMainThread(() =>
            {
                new FucinePath(path).WithItemAtAbsolutePath(
                    token => token.Retire(),
                    sphere => throw new BadRequestException("Cannot delete a sphere."));
            });

            await context.SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Get all spheres at the given path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path to get the spheres at.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "GET", Path = "**path/spheres")]
        public async Task GetSpheresAtPath(IHttpContext context, string path)
        {
            var items = await Dispatcher.RunOnMainThread(() =>
            {
                return new FucinePath(path).WithItemAtAbsolutePath(
                    token => from child in token.Payload.GetSpheres()
                             let json = JsonTranslator.ObjectToJson(child)
                             select json,
                    sphere => throw new BadRequestException("Cannot get spheres of a sphere."));
            });

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        /// <summary>
        /// Gets all tokens at the given path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the sphere to get tokens for.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        [WebRouteMethod(Method = "GET", Path = "**path/tokens")]
        public async Task GetTokensAtPath(IHttpContext context, string path)
        {
            context.Request.QueryString.TryGetValue("payloadType", out var payloadType);
            context.Request.QueryString.TryGetValue("entityId", out var entityId);

            var items = await Dispatcher.RunOnMainThread(() =>
            {
                return new FucinePath(path).WithItemAtAbsolutePath(
                    token => throw new BadRequestException("Cannot get tokens of a token."),
                    sphere => from token in sphere.GetTokens()
                              where !token.Defunct
                              where token.Payload is ElementStack || token.Payload is Situation
                              where payloadType == null || token.PayloadTypeName == payloadType
                              where entityId == null || token.PayloadEntityId == entityId
                              let json = this.TokenToJObject(token)
                              select json);
            });

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        /// <summary>
        /// Creates a token in the sphere.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the sphere.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        /// <exception cref="BadRequestException">The request was malformed.</exception>
        [WebRouteMethod(Method = "POST", Path = "**path/tokens")]
        public async Task CreateTokenAtPath(IHttpContext context, string path)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                return new FucinePath(path).WithItemAtAbsolutePath(
                    token => throw new BadRequestException("Cannot create tokens in a token."),
                    sphere =>
                    {
                        var body = context.ParseBody<JToken>();

                        if (body is JArray)
                        {
                            var items = new List<JToken>();
                            foreach (var item in body)
                            {
                                items.Add(this.CreateSphereToken(sphere, item as JObject));
                            }

                            return JArray.FromObject(items.ToArray());
                        }
                        else if (body is JObject jObj)
                        {
                            return this.CreateSphereToken(sphere, jObj);
                        }

                        throw new BadRequestException("Invalid request body, must be object or array.");
                    });
            });

            await context.SendResponse(HttpStatusCode.Created, result);
        }

        /// <summary>
        /// Deletes all tokens in the sphere.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="path">The path of the sphere.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        [WebRouteMethod(Method = "DELETE", Path = "**path/tokens")]
        public async Task DeleteAllTokensAtPath(IHttpContext context, string path)
        {
            await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSphereByReallyAbsolutePathOrNullSphere(new FucinePath(path));
                if (sphere == null || !sphere.IsValid())
                {
                    throw new NotFoundException($"No sphere found at path \"{path}\".");
                }

                foreach (var token in sphere.GetTokens().ToArray())
                {
                    // Don't delete things we dont care about.
                    // This is mostly for dropzones.
                    if (token.Payload is ElementStack || token.Payload is Situation)
                    {
                        token.Retire();
                    }
                }
            });

            await context.SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Executes the recipe of the situation at the given path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the situation token.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "POST", Path = "**path/execute")]
        public async Task ExecuteSituationAtPath(IHttpContext context, string path)
        {
            var executedRecipeId = await Dispatcher.RunOnMainThread(() =>
            {
                var situation = new FucinePath(path).GetPayload<Situation>();
                if (situation == null)
                {
                    throw new NotFoundException($"No situation found at path \"{path}\".");
                }

                if (situation.StateIdentifier != SecretHistories.Enums.StateEnum.Unstarted)
                {
                    throw new ConflictException($"Situation {situation.VerbId} is not in the correct state to begin a recipe.");
                }

                situation.TryStart();
                if (situation.StateIdentifier == SecretHistories.Enums.StateEnum.Unstarted)
                {
                    throw new ConflictException($"Situation {situation.VerbId} could not begin it's recipe.");
                }

                return situation.FallbackRecipeId;
            });

            await context.SendResponse(HttpStatusCode.OK, new { executedRecipeId });
        }

        /// <summary>
        /// Concludes the situation at the given path.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="path">The path of the situation token.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "POST", Path = "**path/conclude")]
        public async Task ConcludeSituationAtPath(IHttpContext context, string path)
        {
            await Dispatcher.RunOnMainThread(() =>
            {
                var situation = new FucinePath(path).GetPayload<Situation>();
                if (situation == null)
                {
                    throw new NotFoundException($"No situation found at path \"{path}\".");
                }

                if (situation.StateIdentifier != SecretHistories.Enums.StateEnum.Complete)
                {
                    throw new ConflictException($"Situation {situation.VerbId} is not in the correct state to conclude.");
                }

                situation.Conclude();
            });

            await context.SendResponse(HttpStatusCode.OK);
        }

        private JToken CreateSphereToken(Sphere sphere, JObject payload)
        {
            if (!payload.TryGetValue("payloadType", out var payloadType))
            {
                throw new BadRequestException("payloadType is required");
            }

            switch (payloadType.Value<string>())
            {
                case "ElementStack":
                    {
                        var item = payload.ToObject<ElementStackCreationPayload>();
                        item.Validate();
                        var token = item.Create(sphere);
                        return this.TokenToJObject(token);
                    }

                case "Situation":
                    {
                        var item = payload.ToObject<SituationCreationPayload>();
                        item.Validate();
                        var token = item.Create(sphere);
                        return this.TokenToJObject(token);
                    }
            }

            throw new BadRequestException($"Unknown payload type {payloadType}");
        }

        private void UpdateToken(JObject body, Token token)
        {
            JsonTranslator.UpdateObjectFromJson(body, token, false);
            if (JsonTranslator.HasStrategyFor(token.Payload))
            {
                JsonTranslator.UpdateObjectFromJson(body, token.Payload, false);
            }
            else
            {
                Logging.LogTrace("No strategy for token payload type {0}", token.Payload.GetType().FullName);
            }
        }

        private JObject TokenToJObject(Token token)
        {
            var json = new JObject();

            JsonTranslator.ObjectToJson(token, json);

            if (JsonTranslator.HasStrategyFor(token.Payload))
            {
                JsonTranslator.ObjectToJson(token.Payload, json);
            }
            else
            {
                Logging.LogTrace("No strategy for token payload type {0}", token.Payload.GetType().FullName);
            }

            return json;
        }
    }
}
