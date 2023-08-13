namespace CSRestAPI.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ceen;
    using CSRestAPI.Payloads;
    using CSRestAPI.Server.Attributes;
    using CSRestAPI.Server.Exceptions;
    using SecretHistories.Constants;
    using SecretHistories.Entities;
    using SecretHistories.Fucine;
    using SecretHistories.UI;

    /// <summary>
    /// Web request controller for time-related requests.
    /// </summary>
    [WebController(Path = "api/time")]
    public class TimeController
    {
        /// <summary>
        /// Gets the game speed.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task resolving when the request is completed.</returns>
        /// <exception cref="ConflictException">The game is not in a running state.</exception>
        [WebRouteMethod(Method = "GET", Path = "speed")]
        public async Task GetSpeed(IHttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var speed = Dispatcher.RunOnMainThread(() => Watchman.Get<Heart>().GetEffectiveGameSpeed());

            await context.SendResponse(HttpStatusCode.OK, new { speed });
        }

        /// <summary>
        /// Sets the game speed.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task resolving when the request is completed.</returns>
        [WebRouteMethod(Method = "POST", Path = "speed")]
        public async Task SetSpeed(IHttpContext context)
        {
            var payload = context.ParseBody<SetSpeedPayload>();
            payload.Validate();

            await Dispatcher.RunOnMainThread(() =>
            {
                var controlEventArgs = new SpeedControlEventArgs()
                {
                    ControlPriorityLevel = 1,
                    GameSpeed = payload.GameSpeed,
                };
                Watchman.Get<LocalNexus>().SpeedControlEvent.Invoke(controlEventArgs);
            });

            await context.SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Sets the fixed beat time.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task resolving when the request is completed.</returns>
        [WebRouteMethod(Method = "POST", Path = "beat")]
        public async Task ElapseFixedBeat(IHttpContext context)
        {
            var payload = context.ParseBody<FixedBeatPayload>();
            payload.Validate();

            await Dispatcher.RunOnMainThread(() =>
            {
                var heart = Watchman.Get<Heart>();
                heart.Beat(payload.Time, 0);
            });

            await context.SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets the next in-game events.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task resolving when the request is completed.</returns>
        [WebRouteMethod(Method = "GET", Path = "events")]
        public async Task GetNextEvents(IHttpContext context)
        {
            var result = await Dispatcher.RunOnMainThread(() =>
            {
                return new
                {
                    nextCardTime = GetNextCardTime(),
                    nextVerbTime = GetNextVerbTime(),
                };
            });

            await context.SendResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Fast forwards to the next in-game event.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <returns>A task resolving when the request is completed.</returns>
        [WebRouteMethod(Method = "POST", Path = "events/beat")]
        public async Task BeatNextEvent(IHttpContext context)
        {
            var payload = context.ParseBody<BeatNextEventPayload>();
            payload.Validate();

            float timeToBeat = 0f;

            await Dispatcher.RunOnMainThread(() =>
            {
                if (payload.Event == "CardDecay")
                {
                    timeToBeat = GetNextCardTime();
                }
                else if (payload.Event == "RecipeCompletion")
                {
                    timeToBeat = GetNextVerbTime();
                }
                else if (payload.Event == "Either")
                {
                    timeToBeat = Math.Min(GetNextCardTime(), GetNextVerbTime());
                }

                var heart = Watchman.Get<Heart>();
                heart.Beat(timeToBeat, 0);
            });

            await context.SendResponse(HttpStatusCode.OK, new
            {
                timeToBeat,
            });
        }

        // This code courtesy of KatTheFox
        // see: https://github.com/KatTheFox/The-Wheel/blob/main/TheWheel.cs
        private static float GetNextCardTime()
        {
            var elementStacks = Watchman.Get<HornedAxe>().GetExteriorSpheres().Where(x => x.TokenHeartbeatIntervalMultiplier > 0.0f).SelectMany(x => x.GetTokens()).Select(x => x.Payload).OfType<ElementStack>();

            var lowest = float.PositiveInfinity;
            foreach (var stack in elementStacks)
            {
                if (stack.Decays && stack.LifetimeRemaining < lowest && stack.LifetimeRemaining >= 0.2f)
                {
                    lowest = stack.LifetimeRemaining;
                }
            }

            lowest = Math.Min(lowest, GetNextVerbTime());

            return float.IsPositiveInfinity(lowest) ? 0.0f : lowest;
        }

        // This code courtesy of KatTheFox
        // see: https://github.com/KatTheFox/The-Wheel/blob/main/TheWheel.cs
        private static float GetNextVerbTime()
        {
            var verbList = Watchman.Get<HornedAxe>().GetRegisteredSituations();

            if (verbList.Count == 0)
            {
                return 0.0f;
            }

            float lowest = float.PositiveInfinity;
            foreach (Situation verb in verbList)
            {
                if (verb.TimeRemaining < lowest && verb.TimeRemaining >= 0.1f)
                {
                    lowest = verb.TimeRemaining;
                }
            }

            if (float.IsPositiveInfinity(lowest))
            {
                return 0.0f;
            }

            return lowest;
        }
    }
}
