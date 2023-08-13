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
        /// Gets the ID of the containing sphere.
        /// </summary>
        /// <param name="token">The token to get the sphere ID of.</param>
        /// <returns>The id of the containing sphere.</returns>
        [JsonPropertyGetter("sphereId")]
        public string GetSphereId(Token token)
        {
            return token.Sphere.Id;
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
