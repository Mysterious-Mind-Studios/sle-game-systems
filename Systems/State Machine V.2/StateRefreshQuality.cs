using UnityEngine;

namespace FSMSystemV2
{
    public enum StateRefreshQuality
    {
        [Tooltip("Longest frequency when checking state status. Better performance but lower quality. \n (Speed: every second.)")]
        performance,
        [Tooltip("Balanced frequency when checking state status. \n (Speed: every half second.)")]
        balanced,
        [Tooltip("Shortest frequency when checking state status. Less performant but highest quality.\n (Speed: every frame.)")]
        precision
    }
}