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
using System.Collections.Generic;

namespace SLE.Systems.Selection.Collection
{
    /// <summary>
    /// Collection designed for the Selection System. 
    /// 
    /// <para>The public methods automatically handles the selection and deselection of the items added or removed from it. </para>
    /// 
    /// <para>This was also made to filter the multi-selection against whatever object exists in the project based on a supplied delegate condition.<br/>
    /// Since the selection occurs only once in each object, it doesn't accept duplicated references of the same object added in it. </para>
    /// 
    /// <para>NOTE: This Collection works with any implementation of <see cref="ISelectable"/> types, including the interface itself. </para>
    /// </summary>
    public sealed class SelectionList<Selectable> : IReadOnlyCollection<Selectable> where Selectable : ISelectable
    {
        /// <summary>
        /// Initializes a new instance of the class <see cref="SelectionList{Selectable}"/>
        /// </summary>
        public SelectionList()
        {
            _items = new HashSet<Selectable>();
        }

        private HashSet<Selectable> _items;

        /// <summary>
        /// Get the number of the elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        /// <summary>
        /// Add the item and select it.
        /// </summary>
        public void Add(Selectable item)
        {
            if (_items.Add(item))
                item.Select();
        }

        /// <summary>
        /// Remove the item and deselect it.
        /// </summary>
        public void Remove(Selectable item)
        {
            if (_items.Remove(item))
                item.Deselect();
        }

        /// <summary>
        /// Removes all the items deselecting them.
        /// </summary>
        public void Clear()
        {
            foreach (var item in _items)
            {
                item.Deselect();
            }

            _items.Clear();
        }

        /// <summary>
        /// Adds and select the <paramref name="item"/> if the supplied delegate evaluates to true in the item.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        /// <param name="condition"> The condition that will be checked directly in the item. </param>
        public void AddOnly(Selectable item, Predicate<Selectable> condition)
        {
            if (condition(item))
                Add(item);
        }

        /// <summary>
        /// Remove and deselect the <paramref name="item"/> if the supplied delegate evaluates to true in the item.
        /// <para>Note: If the item was not previously selected (what means that it is not in the list) nothing is going to happen. </para>
        /// </summary>        
        /// <param name="item"> The item to remove. </param>
        /// <param name="condition"> The condition that will be checked directly in the item. </param>
        public void RemoveOnly(Selectable item, Predicate<Selectable> condition)
        {
            if (!_items.Contains(item)) return;

            if (condition(item))
                Remove(item);
        }

        /// <summary>
        /// Check if the item is present in the collection.
        /// </summary>
        /// <param name="item">Item to be checked.</param>
        /// <returns>True if collection contains the item.</returns>
        public bool Contains(Selectable item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Check if some item in the collection match the given condition.
        /// </summary>
        /// <param name="item">Item to be checked.</param>
        /// <returns>True if the supplied delegate returns true in any element of the collection.</returns>
        public bool Contains(Predicate<Selectable> match)
        {
            foreach (var item in _items)
            {
                return match(item);
            }

            return false;
        }

        public Selectable[] ToArray()
        {
            int length = _items.Count;

            Selectable[] result = new Selectable[length];

            _items.CopyTo(result);

            return result;
        }

        public IEnumerator<Selectable> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}