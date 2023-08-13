namespace CSRestAPI.Payloads
{
    using CSRestAPI.JsonTranslation;
    using SecretHistories.UI;

    /// <summary>
    /// Translation strategy for the <see cref="Token"/> class.
    /// </summary>
    [JsonTranslatorStrategy]
    [JsonTranslatorTarget(typeof(Token))]
    public class TokenTranslationStrategy
    {
        /// <summary>
        /// Gets the ID of the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The ID of the token.</returns>
        [JsonPropertyGetter("id")]
        public string GetId(Token token)
        {
            return token.PayloadId;
        }

        /// <summary>
        /// Gets the fucine path of the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The full path to the token.</returns>
        [JsonPropertyGetter("path")]
        public string GetTokenPath(Token token)
        {
            return token.Sphere.GetAbsolutePath().Path + "/" + token.PayloadId;
        }

        /// <summary>
        /// Gets the fucine path of the containing sphere.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The full path to the sphere.</returns>
        [JsonPropertyGetter("spherePath")]
        public string GetPath(Token token)
        {
            return token.Sphere.GetAbsolutePath().Path;
        }

        /// <summary>
        /// Gets the type of the token's payload.
        /// </summary>
        /// <param name="token">The token to get the payload type of.</param>
        /// <returns>The token's payload type.</returns>
        [JsonPropertyGetter("payloadType")]
        public string GetPayloadType(Token token)
        {
            return token.PayloadTypeName;
        }
    }
}
