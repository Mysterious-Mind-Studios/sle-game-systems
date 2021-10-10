/**
 *  Source code from Youtube channel: TheScreamingFedora. 
 *  
 *  Extra features and bug fixes by: Erick Luis de Souza.
 *  Code by : Erick Luis de Souza.
 *  
 *  email me at: erickluiss@gmail.com 
 *  for aditional information.
 * 
 */


namespace SelectionSystem
{
    internal static class Constants
    {
        internal static readonly float maxRayTravelDistance = int.MaxValue;
        internal const float _minAcceptableVertexDistance = 0.05f;
        internal const float _ninetyDegreesRotation = 90f;
        internal const float _rayBlockerHeight = -100f;
        internal readonly static int _selectionBoxLayer = 1 << References._rayBlockerCollider.gameObject.layer;
    }
}
