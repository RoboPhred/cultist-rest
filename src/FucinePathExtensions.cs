namespace CSRestAPI
{
    using System;
    using CSRestAPI.Server.Exceptions;
    using SecretHistories.Entities;
    using SecretHistories.Fucine;
    using SecretHistories.Spheres;
    using SecretHistories.UI;

    /// <summary>
    /// Extensions for the <see cref="FucinePath"/> class.
    /// </summary>
    public static class FucinePathExtensions
    {
        /// <summary>
        /// Invokes an action with the item at the specified path.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="path">The path at which to get the item.</param>
        /// <param name="withToken">The funciton to invoke if the item is a token.</param>
        /// <param name="withSphere">The function to invoke if the item is a sphere.</param>
        /// <returns>The return value from either function.</returns>
        /// <exception cref="NotFoundException">The path was invalid, or no item was found.</exception>
        public static T WithItemAtAbsolutePath<T>(this FucinePath path, Func<Token, T> withToken, Func<Sphere, T> withSphere)
        {
            if (!path.IsAbsolute() || path.IsWild() || path.IsRoot())
            {
                throw new NotFoundException("The provided path is not an absolute path.");
            }

            if (path.GetEndingPathPart().Category == FucinePathPart.PathCategory.Token)
            {
                var token = Watchman.Get<HornedAxe>().GetTokenByPath(path);
                if (token == null || !token.IsValid())
                {
                    throw new NotFoundException($"No token found at path \"{path}\".");
                }

                return withToken(token);
            }
            else
            {
                var sphere = Watchman.Get<HornedAxe>().GetSphereByReallyAbsolutePathOrNullSphere(path);
                if (sphere == null || !sphere.IsValid())
                {
                    throw new NotFoundException($"No sphere found at path \"{path}\".");
                }

                return withSphere(sphere);
            }
        }
    }
}
