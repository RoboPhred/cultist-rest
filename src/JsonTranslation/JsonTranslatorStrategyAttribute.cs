namespace CSRestAPI.JsonTranslation
{
    using System;

    /// <summary>
    /// Mark the class as a JSON translator strategy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonTranslatorStrategyAttribute : Attribute
    {
    }
}
