
/*
*  Source code from Youtube channel: TheScreamingFedora. 
*  
*  Extra features and bug fixes by: Erick Luis de Souza.
*  Code by : Erick Luis de Souza.
*  
*  email me at: erickluiss@gmail.com 
*  for aditional information.
* 
*/

namespace SLE.Systems.Selection.Data
{
    internal static class Constants
    {
        internal const float MAX_RAY_TRAVEL_DISTANCE        = int.MaxValue;
        internal const float MIN_ACCEPTABLE_VERTEX_DISTANCE = 0.05f;
        internal const float NINETY_DEG_ROTATION            = 90f;
        internal const float RAY_BLOCKER_HEIGHT             = 100f;
        internal readonly static int SELECTION_BOX_LAYER    = References.RayBlockerCollider.gameObject.layer;
    }
}
