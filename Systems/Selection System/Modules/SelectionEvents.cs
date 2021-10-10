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


using UnityEngine.Events;

namespace SelectionSystem.Modules
{
    /// <summary>
    /// Holds the extra events for the selection / deselection operation.
    /// </summary>
    [System.Serializable]
    public sealed class SelectionEvents
    {
        /// <summary>
        /// Must be invoked when the selectable is selected.
        /// </summary>
        public UnityEvent onSelection;

        /// <summary>
        /// Must be invoked when the selectable is deselected.
        /// </summary>
        public UnityEvent onDeselection;
    }
}