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


using System;
using UnityEngine;
using SelectionSystem.Modules;

namespace SLE.Systems.Selection
{
<<<<<<< Updated upstream
    /// <summary>
    /// The Selectable class can act both as: 
    /// <para> - A single Component ready-to-use so your custom class can use it's functionalities. (SelectionSystem.Component) <br/>
    /// - A base class from which custom classes can Inherit from. (SelectionSystem.Base)</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public sealed class Selector : MonoBehaviour
    {
=======
    [DisallowMultipleComponent]
    public sealed class Selector : MonoBehaviour
    {
        internal static event Action<ISelectable> onDestroy;

>>>>>>> Stashed changes
        [SerializeField]
        private GameObject hLSelection;

        [Space]
        [SerializeField]
        private SelectionEvents selectionEvents = new SelectionEvents();

        /// <summary>
        /// Access the selection events for this selectable instance. (Read Only)
        /// </summary>
        public SelectionEvents SelectionEvents => selectionEvents;

        /// <summary>
        /// Is this object selected?
        /// </summary>
        public bool isSelected { get; private set; } = false;

        private void Start()
        {
            Deselect();
        }

<<<<<<< Updated upstream
=======
        private void OnDestroy()
        {
            ISelectable selectable = GetComponent<ISelectable>();
            onDestroy(selectable);
        }

>>>>>>> Stashed changes
        /// <summary>
        /// Select this object.
        /// </summary>
        public void Select()
        {
            isSelected = true;
            
            selectionEvents.onSelection?.Invoke();
            
            hLSelection?.SetActive(isSelected);
        }

        /// <summary>
        /// Deselect this object.
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            
            selectionEvents.onDeselection?.Invoke();
            
            hLSelection?.SetActive(isSelected);
        }
    }
}