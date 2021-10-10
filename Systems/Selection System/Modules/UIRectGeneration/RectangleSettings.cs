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
using UnityEngine;

namespace SelectionSystem.Modules.UIRectGeneration
{
    /// <summary>
    /// Contains values to apply to the 2D selection box generated on the screen.<br/><br/>
    /// <remarks>
    /// These values can only be applied in the Unity's Inspector.
    /// </remarks>
    /// </summary>
    [Serializable]
    public struct RectangleSettings
    {
        [Space]
        [Header("Selection Box Settings")]
        [SerializeField]
        private Color _rectangleColor;

        [SerializeField]
        private Color _borderColor;

        [SerializeField]
        private float _borderThickness;

        /// <summary>
        /// Current defined color to fill the rectangle.
        /// </summary>
        public Color rectColor { get => _rectangleColor; }

        /// <summary>
        /// Current defined color to the rectangle's border.
        /// </summary>
        public Color borderColor { get => _borderColor; }

        /// <summary>
        /// How thick the border is defined to be.
        /// </summary>
        public float borderThickness { get => _borderThickness; }
    }
}
