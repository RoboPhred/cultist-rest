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
        /// Starts a new legacy.
        /// </summary>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A task that completes when the request is handled.</returns>
        [WebRouteMethod(Method = "POST", Path = "new-legacy")]
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
