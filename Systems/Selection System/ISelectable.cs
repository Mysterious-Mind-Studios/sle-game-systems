/*
 *  Source code from Youtube channel: TheScreamingFedora. 
 *  
 *  Extra features and bug fixes by: Erick Luis de Souza.
 *  Code by : Erick Luis de Souza.
 *  
 *  email me at: erickluiss@gmail.com for aditional information.
 * 
 */

using System;

namespace SLE.Systems.Selection
{
    /// <summary>
    /// This interface makes the <see cref="SelectionHandler"/> recognizes any object as a selectable one. <br/>
    /// </summary>
    public interface ISelectable
    {
        /// <summary> Is the object selected? </summary>
        bool isSelected { get; }

        /// <summary>
        /// Select this object.
        /// </summary>
        void Select();

        /// <summary>
        /// Deselect this object.
        /// </summary>
        void Deselect();
    }
}
