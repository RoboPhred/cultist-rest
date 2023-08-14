namespace CSRestAPI
{
    using System.Threading.Tasks;
    using SecretHistories.UI;

    /// <summary>
    /// Responsible for waiting for the game to settle down after performing events.
    /// </summary>
    public static class Settler
    {
        /// <summary>
        /// Waits for the game to settle.
        /// </summary>
        /// <returns>A task that resolves when the game has no ongoing tasks that are incomplete.</returns>
        /// <remarks>
        /// Settling is defined as having no ongoing token itineraries.
        /// </remarks>
        public static async Task Settle()
        {
            while (await IsSettled() == false)
            {
                Task.Delay(100).Wait();
            }

            // Would be great to do this, but it looks like this wont stop the animations, and the animation will also call Arrive on completed.
            // var itineraries = Watchman.Get<Xamanek>().CurrentItineraries.Values.ToArray();
            // foreach (var itinerary in itineraries)
            // {
            //     itinerary.Arrive(itinerary.GetToken(), new Context(Context.ActionSource.Debug));
            // }
        }

        private static Task<bool> IsSettled()
        {
            // This is kinda hackish.  Autoccultist has a more involved but significantly better way of handling threading.
            return Dispatcher.RunOnMainThread(() => Watchman.Get<Xamanek>().CurrentItineraries.Count == 0);
        }
    }
}
