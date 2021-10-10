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

namespace SelectionSystem.Modules
{
    internal static class MeshGenerator
    {
        public static Mesh GenerateMesh(Vector3[] corners, Vector3[] lines)
        {
            Vector3[] verticies = new Vector3[8];
            int[] triangles = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

            // botton rectangle
            for (int b = 0; b < 4; b++)
            {
                verticies[b] = corners[b];
            }

            // top rectangle
            for (int t = 4; t < 8; t++)
            {
                verticies[t] = corners[t - 4] + lines[t - 4];
            }

            var mesh = new Mesh();
            mesh.vertices = verticies;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
