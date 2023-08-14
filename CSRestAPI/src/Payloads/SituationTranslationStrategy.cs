namespace CSRestAPI.Payloads
{
    using CSRestAPI.JsonTranslation;
    using CSRestAPI.Server.Exceptions;
    using SecretHistories.Commands.SituationCommands;
    using SecretHistories.Entities;
    using SecretHistories.States;
    using SecretHistories.UI;

    /// <summary>
    /// Translation strategy for Situations.
    /// </summary>
    [JsonTranslatorStrategy]
    [JsonTranslatorTarget(typeof(Situation))]
    public class SituationTranslationStrategy
    {
        /// <summary>
        /// Gets the verb id of this situation.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The verb id.</returns>
        [JsonPropertyGetter("verbId")]
        public string GetVerbId(Situation situation)
        {
            return situation.VerbId;
        }

        /// <summary>
        /// Gets the time remaining in the situation's current recipe.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The time remaining.</returns>
        [JsonPropertyGetter("timeRemaining")]
        public float GetTimeRemaining(Situation situation)
        {
            return situation.TimeRemaining;
        }

        /// <summary>
        /// Gets the recipe ID of the situation's fallback recipe.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The fallback recipe ID.</returns>
        [JsonPropertyGetter("recipeId")]
        public string GetFallbackRecipeId(Situation situation)
        {
            if (!situation.FallbackRecipe.IsValid())
            {
                return null;
            }

            return situation.FallbackRecipe.Id;
        }

        /// <summary>
        /// Sets the fallback recipe ID of the situation.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <param name="value">The recipe id.</param>
        /// <exception cref="BadRequestException">The recipe is not found.</exception>
        /// <exception cref="ConflictException">The situation is not in the correct state to begin a recipe.</exception>
        [JsonPropertySetter("recipeId")]
        public void SetFallbackRecipeId(Situation situation, string value)
        {
            var hasRecipe = !string.IsNullOrEmpty(value);

            Recipe recipe = hasRecipe ? Watchman.Get<Compendium>().GetEntityById<Recipe>(value) : null;
            if (hasRecipe && !recipe.IsValid())
            {
                throw new BadRequestException($"Recipe ID {value} not found.");
            }

            if (situation.StateIdentifier != SecretHistories.Enums.StateEnum.Unstarted)
            {
                var nullRecipe = NullRecipe.Create();

                situation.State = SituationState.Rehydrate(SecretHistories.Enums.StateEnum.Unstarted, situation);
                situation.SetRecipeActive(nullRecipe);
                situation.CurrentRecipe = nullRecipe;
            }

            if (!hasRecipe)
            {
                return;
            }

            situation.CommandQueue.RemoveAll(x => x is TryActivateRecipeCommand);
            situation.CommandQueue.Add(new TryActivateRecipeCommand(recipe.Id));
        }

        /// <summary>
        /// Gets the recipe ID of the situation's current recipe.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The current recipe ID.</returns>
        [JsonPropertyGetter("currentRecipeId")]
        public string GetCurrentRecipeId(Situation situation)
        {
            if (!situation.CurrentRecipe.IsValid())
            {
                return null;
            }

            return situation.CurrentRecipe.Id;
        }

        /// <summary>
        /// Gets the situation's state.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The situation's state.</returns>
        [JsonPropertyGetter("state")]
        public string GetState(Situation situation)
        {
            return situation.StateIdentifier.ToString();
        }

        /// <summary>
        /// Gets the situation's icon.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The icon.</returns>
        [JsonPropertyGetter("icon")]
        public string GetIcon(Situation situation)
        {
            return situation.Icon;
        }

        /// <summary>
        /// Gets the situation's label.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The label.</returns>
        [JsonPropertyGetter("label")]
        public string GetLabel(Situation situation)
        {
            return situation.Label;
        }

        /// <summary>
        /// Gets the situation's description.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>The description.</returns>
        [JsonPropertyGetter("description")]
        public string GetDescription(Situation situation)
        {
            return situation.Description;
        }

        /// <summary>
        /// Gets a value indicating if the situation is open.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <returns>A value indicating if the situation is open.</returns>
        [JsonPropertyGetter("open")]
        public bool GetIsOpen(Situation situation)
        {
            return situation.IsOpen;
        }

        /// <summary>
        /// Sets if the situation is open.
        /// </summary>
        /// <param name="situation">The situation.</param>
        /// <param name="value">True to open the situation, False to close it.</param>
        [JsonPropertySetter("open")]
        public void SetIsOpen(Situation situation, bool value)
        {
            if (value)
            {
                situation.OpenAt(situation.Token.Location);
            }
            else
            {
                situation.Close();
            }
        }
    }
}
