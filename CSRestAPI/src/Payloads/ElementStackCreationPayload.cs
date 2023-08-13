namespace CSRestAPI.Payloads
{
    using System.Collections.Generic;
    using CSRestAPI.Server.Exceptions;
    using SecretHistories.Spheres;
    using SecretHistories.UI;

    /// <summary>
    /// A payload for creating an element stack.
    /// </summary>
    public class ElementStackCreationPayload
    {
        /// <summary>
        /// Gets or sets the ID of the element to create a stack of.
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the element to create a stack of.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the mutations to apply to the element stack.
        /// </summary>
        public Dictionary<string, int> Mutations { get; set; } = new();

        /// <summary>
        /// Validates the payload.
        /// </summary>
        /// <exception cref="BadRequestException">The payload data is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.ElementId))
            {
                throw new BadRequestException("ElementId is required");
            }

            if (this.Quantity <= 0)
            {
                throw new BadRequestException("Quantity must be greater than 0");
            }
        }

        /// <summary>
        /// Creates an element stack from the payload.
        /// </summary>
        /// <param name="sphere">The sphere to create the token in.</param>
        /// <returns>The created token.</returns>
        public Token Create(Sphere sphere)
        {
            var token = sphere.ProvisionElementToken(this.ElementId, this.Quantity);
            if (this.Mutations != null && this.Mutations.Count > 0)
            {
                foreach (var pair in this.Mutations)
                {
                    (token.Payload as ElementStack).SetMutation(pair.Key, pair.Value, false);
                }
            }

            return token;
        }
    }
}
