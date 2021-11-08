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
    [DisallowMultipleComponent]
    public sealed class Selector : MonoBehaviour
    {
        internal static event Action<ISelectable> onDestroy;

        [SerializeField]
        private GameObject selectionHL;

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

        private void OnDestroy()
        {
            ISelectable selectable = GetComponent<ISelectable>();
            onDestroy(selectable);
        }

        /// <summary>
        /// Select this object.
        /// </summary>
        public void Select()
        {
            isSelected = true;
            
            selectionEvents.onSelection?.Invoke();
            
            selectionHL?.SetActive(isSelected);
        }

        /// <summary>
        /// Deselect this object.
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            
            selectionEvents.onDeselection?.Invoke();
            
            selectionHL?.SetActive(isSelected);
        }
    }
}