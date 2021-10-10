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

namespace SelectionSystem.Base
{
    /// <summary>
    /// The Selectable class can act both as: 
    /// <para> - A single Component ready-to-use so your custom class can use it's functionalities. (SelectionSystem.Component) <br/>
    /// - A base class from which custom classes can Inherit from. (SelectionSystem.Base)</para>
    /// Opposite to the component class of the Selectable, this one already implements the interface ISelectable. When it's inherited all the logic must be written
    /// down.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public abstract class Selectable : MonoBehaviour, ISelectable
    {
        /// <inheritdoc/>
        public abstract bool isSelected
        {
            get;
        }

        /// <inheritdoc/>
        public abstract void Select();

        /// <inheritdoc/>
        public abstract void Deselect();
    }
}
