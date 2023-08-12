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
        /// Gets the payload ID of the token.
        /// </summary>
        /// <param name="token">The token to get the payload ID for.</param>
        /// <returns>The token payload id.</returns>
        [JsonPropertyGetter("payloadId")]
        public string GetPayloadId(Token token)
        {
            return token.PayloadId;
        }

        /// <summary>
        /// Gets the ID of the entity of the token's payload.
        /// </summary>
        /// <param name="token">The token to get the payload entity id of.</param>
        /// <returns>The token's payload entity id.</returns>
        [JsonPropertyGetter("payloadEntityId")]
        public string GetPayloadEntityId(Token token)
        {
            return token.PayloadEntityId;
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

        /// <summary>
        /// Gets the quantity of the token.
        /// </summary>
        /// <param name="token">The token to get the quantity of.</param>
        /// <returns>The token's quantity.</returns>
        [JsonPropertyGetter("quantity")]
        public int GetQuantity(Token token)
        {
            return token.Quantity;
        }

        /// <summary>
        /// Gets a value indicating if the token is shrouded.
        /// </summary>
        /// <param name="token">The token to get the shrouded status of.</param>
        /// <returns>The token's shrouded status.</returns>
        [JsonPropertyGetter("shrouded")]
        public bool GetShrouded(Token token)
        {
            return token.Shrouded;
        }

        /// <summary>
        /// Sets the token's shrouded status.
        /// </summary>
        /// <param name="token">The token to get the shrouded status of.</param>
        /// <param name="shrouded">The shrouded status to set.</param>
        [JsonPropertySetter("shrouded")]
        public void SetShrouded(Token token, bool shrouded)
        {
            if (shrouded)
            {
                token.Shroud();
            }
            else
            {
                token.Unshroud();
            }
        }
    }
}
