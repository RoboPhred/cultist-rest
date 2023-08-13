namespace CSRestAPI.Payloads
{
    using CSRestAPI.Server.Exceptions;

    /// <summary>
    /// Represents a payload for setting a fixed beat time.
    /// </summary>
    public class FixedBeatPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedBeatPayload"/> class.
        /// </summary>
        public FixedBeatPayload()
        {
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The fixed beat time.</value>
        public float Time { get; set; }

        /// <summary>
        /// Validates the payload.
        /// </summary>
        /// <exception cref="BadRequestException">The payload is invalid.</exception>
        public void Validate()
        {
            if (this.Time <= 0)
            {
                throw new BadRequestException("Invalid time.");
            }
        }
    }
}
