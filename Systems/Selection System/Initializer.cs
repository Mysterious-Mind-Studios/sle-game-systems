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


using UnityEngine;
using SelectionSystem.Components;

namespace SelectionSystem
{
    internal static class Initializer
    {
        public static void Run()
        {
            var newGOGenerated = GameObject.CreatePrimitive(PrimitiveType.Quad);

            References._rayBlockerCollider = newGOGenerated.GetComponent<MeshCollider>();
            var mesh = ((MeshCollider)References._rayBlockerCollider).sharedMesh;
            mesh.Optimize();

            References._rayBlockerCollider.gameObject.name = "Auto-generated: Ray blocker";
            References._rayBlockerCollider.gameObject.layer = 31;
            References._rayBlockerCollider.gameObject.isStatic = true;

            //Adjust position and scale in 3D world.
            References._rayBlockerCollider.transform.position = Vector3.up * Constants._rayBlockerHeight;
            References._rayBlockerCollider.transform.localScale = new Vector2(100000, 100000);
            References._rayBlockerCollider.transform.rotation = Quaternion.Euler(Vector3.right * Constants._ninetyDegreesRotation);

            UnityEngine.Object.Destroy(References._rayBlockerCollider.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(References._rayBlockerCollider.GetComponent<MeshRenderer>());

            var selectionHandlers = GameObject.FindObjectsOfType<SelectionHandler>();

            for (int i = selectionHandlers.Length - 1; i >= 0; i--)
            {
                // Reset position first. (Safety purposes)
                selectionHandlers[i].transform.position = Vector3.zero; 

                // In order to work make the selection handler(game object) goes to the same height of the ray blocker.
                selectionHandlers[i].transform.position = Vector3.up * Constants._rayBlockerHeight;
            }
        }
    }
}
