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


using UnityEngine;
using SLE.Systems.Selection.Data;

namespace SLE.Systems.Selection
{
    internal static class Initializer
    {
        public static void Run()
        {
            var newGOGenerated = GameObject.CreatePrimitive(PrimitiveType.Quad);

            References.RayBlockerCollider = newGOGenerated.GetComponent<MeshCollider>();
            var mesh = ((MeshCollider)References.RayBlockerCollider).sharedMesh;
            mesh.Optimize();

            References.RayBlockerCollider.gameObject.name = "Auto-generated: Ray blocker";
            References.RayBlockerCollider.gameObject.layer = 1 << 31;
            References.RayBlockerCollider.gameObject.isStatic = true;

            //Adjust position and scale in 3D world.
            References.RayBlockerCollider.transform.position = Vector3.down * Constants.RAY_BLOCKER_HEIGHT;
            References.RayBlockerCollider.transform.localScale = new Vector2(100000, 100000);
            References.RayBlockerCollider.transform.rotation = Quaternion.Euler(Vector3.right * Constants.NINETY_DEG_ROTATION);

            Object.Destroy(References.RayBlockerCollider.GetComponent<MeshFilter>());
            Object.Destroy(References.RayBlockerCollider.GetComponent<MeshRenderer>());

            var selectionHandlers = Object.FindObjectsOfType<SelectionHandler>();

            for (int i = selectionHandlers.Length - 1; i >= 0; i--)
            {
                // Reset position first. (Safety purposes)
                selectionHandlers[i].transform.position = Vector3.zero;

                // In order to work make the selection handler(game object) goes to the same height of the ray blocker.
                selectionHandlers[i].transform.position = Vector3.down * Constants.RAY_BLOCKER_HEIGHT;
            }
        }
    }
}
