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

namespace SLE.Systems.Selection
{
    /// <summary>
    /// Interface to access the main properties of the selection highlight of the objects.
    /// </summary>
    public interface ISelectionHighlight
    {
        /// <summary>
        /// Get the current color of the selection highlight.
        /// </summary>
        Color color { get; }

        /// <summary>
        /// Get the current size of the selection highlight.
        /// </summary>
        float size { get; }

        /// <summary>
        /// Set the selection highlight active.
        /// </summary>
        void Activate();

        /// <summary>
        /// Set the selection highlight inactive.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Define a new color to the selection highlight.
        /// </summary>
        void SetHighlightColor(Color color);
    }
}