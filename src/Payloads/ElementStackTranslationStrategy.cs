namespace CSRestAPI.Payloads
{
    using CSRestAPI.JsonTranslation;
    using Newtonsoft.Json.Linq;
    using SecretHistories.UI;

    /// <summary>
    /// Translation strategy for <see cref="ElementStack"/>.
    /// </summary>
    [JsonTranslatorStrategy]
    [JsonTranslatorTarget(typeof(ElementStack))]
    public class ElementStackTranslationStrategy
    {
        /// <summary>
        /// Sets the quantity of the element stack.
        /// </summary>
        /// <param name="elementStack">The element stack to set.</param>
        /// <param name="quantity">The quantity to set.</param>
        [JsonPropertySetter("quantity")]
        public void SetQuantity(ElementStack elementStack, int quantity)
        {
            elementStack.SetQuantity(quantity);
        }

        /// <summary>
        /// Gets the time remaining in seconds.
        /// </summary>
        /// <param name="elementStack">The element stack to get the lifetime remaining of.</param>
        /// <returns>The lifetime remaining of the ElementStack.</returns>
        [JsonPropertyGetter("lifetimeRemaining")]
        public float GetLifetimeRemaining(ElementStack elementStack)
        {
            return elementStack.LifetimeRemaining;
        }

        /// <summary>
        /// Gets the aspects of the element stack.
        /// </summary>
        /// <param name="elementStack">The element stack to get the aspects of.</param>
        /// <returns>The aspects of the element stack.</returns>
        [JsonPropertyGetter("aspects")]
        public JObject GetAspects(ElementStack elementStack)
        {
            var obj = new JObject();
            foreach (var pair in elementStack.GetAspects(true))
            {
                obj[pair.Key] = pair.Value;
            }

            return obj;
        }

        /// <summary>
        /// Gets the mutations of the element stack.
        /// </summary>
        /// <param name="elementStack">The element stack to get the mutations of.</param>
        /// <returns>The mutations of the element stack.</returns>
        [JsonPropertyGetter("mutations")]
        public JObject GetMutations(ElementStack elementStack)
        {
            var obj = new JObject();
            foreach (var pair in elementStack.Mutations)
            {
                obj[pair.Key] = pair.Value;
            }

            return obj;
        }

        /// <summary>
        /// Sets the mutations of the element stack.
        /// </summary>
        /// <param name="elementStack">The element stack to set the mutations of.</param>
        /// <param name="mutations">The mutations to set.</param>
        [JsonPropertySetter("mutations")]
        public void SetMutations(ElementStack elementStack, JObject mutations)
        {
            foreach (var pair in mutations)
            {
                elementStack.SetMutation(pair.Key, pair.Value.ToObject<int>(), false);
            }
        }

        [JsonPropertyGetter("elementId")]
        public string GetElementId(ElementStack elementStack)
        {
            return elementStack.Element.Id;
        }

        [JsonPropertyGetter("label")]
        public string GetLabel(ElementStack elementStack)
        {
            return elementStack.Label;
        }

        [JsonPropertyGetter("description")]
        public string GetDescription(ElementStack elementStack)
        {
            return elementStack.Description;
        }

        [JsonPropertyGetter("icon")]
        public string GetIcon(ElementStack elementStack)
        {
            return elementStack.Icon;
        }

        [JsonPropertyGetter("uniquenessGroup")]
        public string GetUniquenessGroup(ElementStack elementStack)
        {
            return elementStack.UniquenessGroup;
        }

        [JsonPropertyGetter("decays")]
        public bool GetDecays(ElementStack elementStack)
        {
            return elementStack.Decays;
        }

        [JsonPropertyGetter("metafictional")]
        public bool GetMetafictional(ElementStack elementStack)
        {
            return elementStack.Metafictional;
        }

        [JsonPropertyGetter("unique")]
        public bool GetUnique(ElementStack elementStack)
        {
            return elementStack.Unique;
        }
    }
}
