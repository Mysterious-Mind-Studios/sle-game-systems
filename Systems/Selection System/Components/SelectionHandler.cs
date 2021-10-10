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

using System;
using System.Collections;
using UnityEngine;
using SelectionSystem.DesignedCollection;
using SelectionSystem.Modules.MeshGeneration;
using SelectionSystem.Modules.UIRectGeneration;

namespace SelectionSystem.Components
{
    /// <summary>
    /// The <see cref="SelectionHandler"/> handles the major part of the entire <seealso cref="SelectionSystem">System</seealso>. <br/>
    /// In order to this to recoginize any selectable object, it's needed to implement <seealso cref="ISelectable"/> interface.
    /// <para>The <seealso cref="SelectionSystem">System</seealso> already brings to you a ready-to-use <seealso cref="Selectable">Component</seealso> with all functionalities needed implemented. <br/>
    /// But it does not implements <seealso cref="ISelectable"/> for flexibility reason. </para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshCollider))]
    public sealed class SelectionHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The camera that will be used to cast the selection box. If none is assigned, the Camera.main will be used instead.")]
        private Camera _camera;

        [Space]
        [SerializeField]
        private RectangleSettings _uiRectSettings = new RectangleSettings();

        private Vector3 currentCursorPosition = Vector3.zero;

        private MeshCollider _meshCollider;

        private Predicate<ISelectable> _multiSelectionRule = null;

        private WaitForSeconds shortDelay = new WaitForSeconds(0.1f);
        
        [NonSerialized]
        private Vector3 onClickCursorPosition;
        [NonSerialized]
        private bool boxSelecting = false;
        [NonSerialized]
        private bool shiftWasPressedLastSelection = false;

        /// <summary>
        /// The camera used in selection operations.
        /// </summary>
        public Camera camera
        {
            get
            {
                if (!_camera)
                    _camera = Camera.main;

                return _camera;
            }
            set
            {
                _camera = value;
            }
        }

        /// <summary>
        /// Access all the current selected objects. (Read Only)
        /// </summary>
        public SelectionList<ISelectable> currentSelection { get; } = new SelectionList<ISelectable>();

        private void Awake()
        {
            if (!_meshCollider)
                _meshCollider = GetComponent<MeshCollider>();

            Initializer.Run();
        }

        private void OnGUI()
        {
            if (boxSelecting)
            {
                var uiRectangle = RectangleGenerator.GetScreenRect(onClickCursorPosition, currentCursorPosition);

                RectangleGenerator.DrawScreenRect(uiRectangle, _uiRectSettings.rectColor);
                RectangleGenerator.DrawScreenRectBorder(uiRectangle, _uiRectSettings.borderThickness, _uiRectSettings.borderColor);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out ISelectable selectable)) return;

            switch (_multiSelectionRule)
            {
                case null:
                    currentSelection.Add(selectable);
                    break;

                default:
                    currentSelection.AddOnly(selectable, _multiSelectionRule);
                    break;
            }
        }

        private void Update()
        {
            currentCursorPosition = InputHelper.GetCursorPosition();

            /* 

             Because shift key most of the times represents the switch for a multiple selection attempt 
             It's by default checked instead of a generic key (Version 1.0.0.0). 
             In the future it might be changed to save a some sort of generic key, making it possible to check for a key based on the project's setting instead.

            */

            shiftWasPressedLastSelection = InputHelper.ShiftKeyIsPressed();
        }

        private IEnumerator DisableMeshCollider() 
        {
            yield return shortDelay;
            _meshCollider.enabled = false; 
        }

        private void DeselectAll()
        {
            currentSelection.Clear();
        }

        private void HandleSingleSelection(ISelectable selectable)
        {
            if (selectable == null)
                return;

            if (shiftWasPressedLastSelection)
            {
                if (_multiSelectionRule != null)
                {
                    currentSelection.AddOnly(selectable, _multiSelectionRule);
                    return;
                }
            }

            currentSelection.Add(selectable);
            Selectable.onDestroy += AutoRemove;
        }

        private void AutoRemove(ISelectable obj)
        {
            currentSelection.Remove(obj);
        }

        private bool CheckIfIsMultiSelecting()
        {
            return (onClickCursorPosition - currentCursorPosition).magnitude > 0;
        }

        private void OnLeftMousePressed()
        {
            onClickCursorPosition = currentCursorPosition;
        }

        private void OnLeftMouseHeld()
        {
            boxSelecting = CheckIfIsMultiSelecting();
        }

        private void OnLeftMouseReleased()
        {
            if (!shiftWasPressedLastSelection)
                DeselectAll();

            if (boxSelecting)                       // Multi-selection
            {
                if (SelectionBoxGenerator.Generate(onClickCursorPosition, currentCursorPosition, camera, ref _meshCollider))
                    StartCoroutine(DisableMeshCollider());

                // Check the multiple selection logic on the OnTriggerEnter(Collider) Method.
            }
            else                                    // Single-selection
            {
                Ray ray = camera.ScreenPointToRay(currentCursorPosition);

                if (!Physics.Raycast(ray, out var hit, Constants.maxRayTravelDistance)) return;

                if (!hit.collider.TryGetComponent(out ISelectable selectable)) return;

                HandleSingleSelection(selectable);
            }

            boxSelecting = false;
        }

        /// <summary>
        /// Automatically handle the selection process based on mouse button press events.
        /// <para>It switches automatically between modes based on active Input System.</para>
        /// </summary>
        public void ProcessSelections()
        {
            if (InputHelper.LeftMouseButtonWasPressed())
                OnLeftMousePressed();

            if (InputHelper.LeftMouseButtonIsHold())
                OnLeftMouseHeld();

            if (InputHelper.LeftMouseButtonWasReleased())
                OnLeftMouseReleased();

            print(currentSelection.Count);
        }

        /// <summary>
        /// Sets a new condition that will be verified when trying to select more than one object. (Either in sequence or at once) <br/>
        /// If it's <keyworkd>null</keyworkd> no condition will be verified and everything will be able to be multi selected.
        /// </summary>
        /// <param name="condition"> 
        /// The specified method that will be invoked on <see cref="ISelectable">Selectable</see> types to verify if it can be selected or not.
        /// </param>
        public void DefineMultiSelectionRule(Predicate<ISelectable> condition = null)
        {
            _multiSelectionRule = condition == null ? null : condition;
        }
    }
}
