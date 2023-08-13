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
    using SecretHistories.Spheres;
    using SecretHistories.UI;

    /// <summary>
    /// A controller dealing with Spheres.
    /// </summary>
    [WebController(Path = "api/spheres")]
    internal class SpheresController
    {
        /// <summary>
        /// Gets all spheres.
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
        /// Gets all tokens in a sphere.
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

                context.Request.QueryString.TryGetValue("payloadType", out var payloadType);
                context.Request.QueryString.TryGetValue("entityId", out var entityId);

                return from token in sphere.GetTokens()
                       where !token.Defunct
                       where payloadType == null || token.PayloadTypeName == payloadType
                       where entityId == null || token.PayloadEntityId == entityId
                       let json = this.TokenToJObject(token)
                       select json;
            });

            await context.SendResponse(HttpStatusCode.OK, items);
        }

        /// <summary>
        /// Creates a token in the sphere.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="sphereId">The ID of the sphere.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        /// <exception cref="BadRequestException">The request was malformed.</exception>
        [WebRouteMethod(Method = "PUT", Path = ":sphereId/tokens")]
        public async Task PutSphereToken(IHttpContext context, string sphereId)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSpheres().FirstOrDefault(x => x.Id == sphereId);
                if (!sphere)
                {
                    throw new NotFoundException("No sphere with the given ID exists.");
                }

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

            await context.SendResponse(HttpStatusCode.Created, result);
        }

        /// <summary>
        /// Gets a token in a sphere by ID.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="sphereId">The sphere id.</param>
        /// <param name="payloadId">The payload id.</param>
        /// <returns>A task that resolves when the request is complete.</returns>
        /// <exception cref="NotFoundException">THe sphere or token was not found.</exception>
        [WebRouteMethod(Method = "GET", Path = ":sphereId/tokens/:payloadId")]
        public async Task GetSphereTokenById(IHttpContext context, string sphereId, string payloadId)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSpheres().FirstOrDefault(x => x.Id == sphereId);
                if (!sphere)
                {
                    throw new NotFoundException("No sphere with the given ID exists.");
                }

                var token = sphere.GetTokens().FirstOrDefault(x => x.PayloadId == payloadId);
                if (!token)
                {
                    throw new NotFoundException("No token with the given ID exists.");
                }

                return JsonTranslator.ObjectToJson(token);
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Modifies a token in a sphere.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="sphereId">The ID of the sphere.</param>
        /// <param name="payloadId">The ID of the payload.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        [WebRouteMethod(Method = "PUT", Path = ":sphereId/tokens/:payloadId")]
        /// <exception cref="NotFoundException">THe sphere or token was not found.</exception>
        /// <exception cref="BadRequestException">The request was malformed.</exception>
        public async Task PutSphereTokenById(IHttpContext context, string sphereId, string payloadId)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSpheres().FirstOrDefault(x => x.Id == sphereId);
                if (!sphere)
                {
                    throw new NotFoundException("No sphere with the given ID exists.");
                }

                var token = sphere.GetTokens().FirstOrDefault(x => x.PayloadId == payloadId);
                if (!token)
                {
                    throw new NotFoundException("No token with the given ID exists.");
                }

                var body = context.ParseBody<JObject>();
                JsonTranslator.ValidateJsonUpdate(body, token);
                JsonTranslator.UpdateObjectFromJson(body, token);

                return JsonTranslator.ObjectToJson(token);
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Deletes all tokens in the sphere.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="sphereId">The ID of the sphere.</param>
        /// <returns>A task that resolves once the request is completed.</returns>
        /// <exception cref="NotFoundException">The sphere was not found.</exception>
        [WebRouteMethod(Method = "DELETE", Path = ":sphereId/tokens")]
        public async Task DeleteSphereContents(IHttpContext context, string sphereId)
        {
            await Dispatcher.RunOnMainThread(() =>
            {
                var sphere = Watchman.Get<HornedAxe>().GetSpheres().FirstOrDefault(x => x.Id == sphereId);
                if (!sphere)
                {
                    throw new NotFoundException("No sphere with the given ID exists.");
                }

                foreach (var token in sphere.GetTokens().ToArray())
                {
                    token.Retire();
                }
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
