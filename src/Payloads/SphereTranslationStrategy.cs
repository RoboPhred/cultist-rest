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
        /// Gets the path for the sphere.
        /// </summary>
        /// <param name="sphere">The sphere to get the path of.</param>
        /// <returns>The sphere path.</returns>
        [JsonPropertyGetter("path")]
        public string GetPath(Sphere sphere)
        {
            return sphere.GetAbsolutePath().Path;
        }

        /// <summary>
        /// Gets the category of the sphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <returns>The sphere category.</returns>
        [JsonPropertyGetter("category")]
        public string GetCategory(Sphere sphere)
        {
            return sphere.SphereCategory.ToString();
        }
    }
}
