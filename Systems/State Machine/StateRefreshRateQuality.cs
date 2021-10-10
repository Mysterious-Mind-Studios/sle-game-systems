using UnityEngine;

namespace StateMachineSystem
{
    public enum StateRefreshRateQuality
    {
        [Tooltip("Longest frequency when checking state status. Better performance but lower quality.")]
        performance,
        [Tooltip("Balanced frequency when checking state status.")]
        balanced,
        [Tooltip("Shortest frequency when checking state status. Less performant but highest quality.")]
        precision
    }
}