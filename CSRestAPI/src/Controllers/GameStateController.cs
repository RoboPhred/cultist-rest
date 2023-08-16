namespace CSRestAPI.Controllers
{
    using System.Threading.Tasks;
    using Ceen;
    using CSRestAPI.Payloads;
    using CSRestAPI.Server.Attributes;
    using Roost;
    using SecretHistories.Entities;
    using SecretHistories.Infrastructure.Persistence;
    using SecretHistories.Services;
    using SecretHistories.UI;

    /// <summary>
    /// Controller for interacting with the game state as a whole.
    /// </summary>
    [WebController(Path = "api/game-state")]
    public class GameStateController
    {
        /// <summary>
        /// Hydrates the game state in a new game.
        /// </summary>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A task that completes when the request is handled.</returns>
        [WebRouteMethod(Method = "PUT")]
        public async Task HydrateGameState(IHttpContext context)
        {
            var payload = context.ParseBody<HydrateGameStatePayload>();
            payload.Validate();

            await Dispatcher.RunOnMainThread(() =>
            {
                var stageHand = Watchman.Get<StageHand>();

                // Switch scenes without using LoadGameOnTabletop.
                // Bit janky, but we would need to await the fades, and LoadGameOnTabletop doesn't return a task.
                stageHand.UsePersistenceProvider(payload.Provider);

                // Roslyn says 'new object[]' can be simplified to '[]'.  Don't believe its lies.  Apparently our csproj doesn't support that.
                typeof(StageHand).GetMethodInvariant("SceneChange").Invoke(stageHand, new object[] { Watchman.Get<Compendium>().GetSingleEntity<Dictum>().PlayfieldScene, false });
            });

            await Settler.AwaitGameReady();

            await Settler.AwaitSettled();

            await context.SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets the active legacy, if any.
        /// </summary>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A task that completes when the request is handled.</returns>
        [WebRouteMethod(Method = "GET", Path = "legacy")]
        public async Task GetLegacy(IHttpContext context)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                string legacyId = null;
                string legacyLabel = null;
                var stageHand = Watchman.Get<StageHand>();
                if (stageHand.SceneIsActive("S4Tabletop"))
                {
                    var protag = Watchman.Get<Stable>().Protag();
                    legacyId = protag.ActiveLegacy.Id;
                    legacyLabel = protag.ActiveLegacy.Label;
                }

                return new
                {
                    legacyId,
                    legacyLabel,
                };
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Starts a new legacy.
        /// </summary>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A task that completes when the request is handled.</returns>
        [WebRouteMethod(Method = "PUT", Path = "legacy")]
        public async Task StartNewLegacy(IHttpContext context)
        {
            var payload = context.ParseBody<StartNewLegacyPayload>();
            payload.Validate();

            await Dispatcher.RunOnMainThread(() =>
            {
                var provider = new FreshGameProvider(payload.Legacy);
                var stageHand = Watchman.Get<StageHand>();

                // Switch scenes without using LoadGameOnTabletop.
                // Bit janky, but we would need to await the fades, and LoadGameOnTabletop doesn't return a task.
                stageHand.UsePersistenceProvider(provider);

                // Roslyn says 'new object[]' can be simplified to '[]'.  Don't believe its lies.  Apparently our csproj doesn't support that.
                typeof(StageHand).GetMethodInvariant("SceneChange").Invoke(stageHand, new object[] { Watchman.Get<Compendium>().GetSingleEntity<Dictum>().PlayfieldScene, false });
            });

            await Settler.AwaitGameReady();
            await Settler.AwaitSettled();

            await context.SendResponse(HttpStatusCode.OK);
        }
    }
}
