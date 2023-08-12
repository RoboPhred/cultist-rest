namespace CSRestAPI.Payloads
{
    using CSRestAPI.JsonTranslation;
    using SecretHistories.Spheres;

    /// <summary>
    /// Translates between a <see cref="Sphere"/> and JSON.
    /// </summary>
    [JsonTranslatorStrategy]
    [JsonTranslatorTarget(typeof(Sphere))]
    public sealed class SphereTranslationStrategy
    {
        /// <summary>
        /// Gets the ID for the sphere.
        /// </summary>
        /// <param name="sphere">The sphere to get the ID of.</param>
        /// <returns>The Sphere ID.</returns>
        [JsonPropertyGetter("id")]
        public string GetId(Sphere sphere)
        {
            return sphere.Id;
        }

        /// <summary>
        /// Gets the path for the sphere.
        /// </summary>
        /// <param name="sphere">The sphere to get the path of.</param>
        /// <returns>The sphere path.</returns>
        [JsonPropertyGetter("path")]
        public string GetPath(Sphere sphere)
        {
            return sphere.GetWildPath().Path;
        }
    }
}
