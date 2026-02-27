using UnityEngine;

namespace SnakePrototype.Core
{
    /// <summary>
    /// Base interface for all game services managed by the ServiceLocator.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Called when the service is registered and initialized.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called when the service is deregistered or the game shuts down.
        /// </summary>
        void Shutdown();
    }
}
