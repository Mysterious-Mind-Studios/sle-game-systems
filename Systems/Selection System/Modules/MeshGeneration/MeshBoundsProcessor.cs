
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

namespace SLE.Systems.Selection.Modules.MeshGeneration
{
    internal static class MeshBoundsProcessor
    {
        public static Vector2[] GetBoundaries(Vector2 position1, Vector2 position2)
        {
            Vector2 newPos1, newPos2, newPos3, newPos4;

            if (position1.x < position2.x)
            {
                if (position1.y > position2.y)
                {
                    newPos1 = position1;
                    newPos2 = new Vector2(position2.x, position1.y);
                    newPos3 = new Vector2(position1.x, position2.y);
                    newPos4 = position2;
                }
                else
                {
                    newPos1 = new Vector2(position1.x, position2.y);
                    newPos2 = position2;
                    newPos3 = position1;
                    newPos4 = new Vector2(position2.x, position1.y);
                }
            }
            else
            {
                if (position1.y > position2.y)
                {
                    newPos1 = new Vector2(position2.x, position1.y);
                    newPos2 = position1;
                    newPos3 = position2;
                    newPos4 = new Vector2(position1.x, position2.y);
                }
                else
                {
                    newPos1 = position2;
                    newPos2 = new Vector2(position1.x, position2.y);
                    newPos3 = new Vector2(position2.x, position1.y);
                    newPos4 = position1;
                }
            }

            Vector2[] corners = { newPos1, newPos2, newPos3, newPos4 };
            return corners;
        }
    }
}
