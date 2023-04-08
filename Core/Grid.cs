using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hitbox.GridFramework
{
    /// <summary>
    /// Responsible for storing and handling all of the data inside the grid.
    /// Can be inherited from to implement custom behaviour.
    /// </summary>
    public class Grid
    {
        #region --- VARIABLES ---
        
        /// <summary>
        /// All of the elements contained within the grid.
        /// </summary>
        protected readonly Dictionary<Vector2Int, GridElement> elements = new ();
        
        /// <summary>
        /// Returns an array of all the elements contained in the grid with no duplicates.
        /// </summary>
        public IEnumerable<GridElement> AllElements => new HashSet<GridElement>(elements.Values).ToArray();
        
        // SERIALIZATION DATA
        private GridElement[] elementData;

        #region - EVENTS -
        
        /// <summary>
        /// Invoked whenever an element is updated within the grid.
        /// </summary>
        public event Action<GridElement> GridUpdated;

        #endregion

        #endregion
        
        #region --- METHODS ---

        #region Manipulation

        #region Insertion

        /// <summary>
        /// Insert an element at a given position.
        /// </summary>
        /// <param name="element">element to insert</param>
        /// <param name="position">position on the grid to insert the element</param>
        /// <param name="combineElements">whether elements should be combined if cell at position is not empty.</param>
        public bool InsertElementAtPosition(GridElement element, Vector2Int position, bool combineElements = true)
        {
            RemoveElement(element); // Removing Element from this Grid, if it's inside it.
            
            if (!CanInsertAtPosition(position, element)) // Checking the element 
            {
                if(element.takenPositions.Length > 0)
                    InsertElementAtPosition(element, element.takenPositions[0]);
                
                return false;
            }

            List<Vector2Int> elementPositions = new ();
            for (int y = position.y; y < position.y + element.Size.y; y++)
            {
                for (int x = position.x; x < position.x + element.Size.x; x++)
                {
                    Vector2Int offsetPos = new (x, y);
                    
                    // Try to combine elements if possible.
                    if (combineElements && ElementAtPosition(offsetPos))
                    {
                        if(elements[offsetPos].profile == element.profile)
                            (elements[offsetPos], element) = elements[offsetPos].profile.TryCombineElements(elements[offsetPos], element);

                        if (element != null) return false;
                        
                        elements[offsetPos].profile.Updated();
                        return true;
                    }
                    
                    elementPositions.Add(offsetPos);
                }
            }

            if (elementPositions.Count == 0) return false; // Element was not inserted.

            foreach (Vector2Int slot in elementPositions)
            {
                elements.Add(slot, element);
            }
            
            element.SetGrid(this); // Parenting element to this grid.
            element.takenPositions = elementPositions.ToArray();

            GridUpdated?.Invoke(element);
            
            return true;
        }

        #endregion

        #region Removal
        
        /// <summary>
        /// Removes and returns the element at the given position.
        /// </summary>
        /// <returns>GridElement removed or null if nothing found.</returns>
        public GridElement RemoveElementAtPosition(Vector2Int position)
        {
            GridElement foundItem = elements[position];

            if (foundItem == null) return null;
            
            RemoveElement(foundItem);

            return foundItem;
        }
        
        /// <summary>
        /// Removes the given element from the grid.
        /// </summary>
        /// <param name="element">target element to remove from the grid</param>
        /// <returns>true if the element was successfully removed</returns>
        public virtual bool RemoveElement(GridElement element)
        {
            if (!element.InGrid(this)) return false; // Stop if element not in this grid.
            if (element.takenPositions == null) return false; // Stop if element has no taken positions.
            
            // Removing all references to the item in this grid, but not from the element itself.
            foreach (Vector2Int slot in element.takenPositions)
            {
                elements.Remove(slot);
            }
            
            GridUpdated?.Invoke(element);

            return true;
        }

        /// <summary>
        /// Checks if the given element is located within the grid.<para/>This is search is of O(N) complexity.
        /// </summary>
        /// <param name="element">Search target</param>
        public bool ContainsElement(GridElement element) => elements.ContainsValue(element);

        /// <summary>
        /// Checks if the given cell is taken by another element.
        /// </summary>
        /// <param name="position">The position in the grid to check.</param>
        /// <returns>true if element has been found at position.</returns>
        public bool ElementAtPosition(Vector2Int position) => elements.ContainsKey(position);

        /// <summary>
        /// Retrieves the grid element located at the given position.
        /// </summary>
        /// <param name="position">position to get element from.</param>
        /// <returns>element at the given position, or null if not found.</returns>
        public GridElement GetElementAtPosition(Vector2Int position) => elements[position];

        #endregion

        #endregion

        #region Serialization

        public void OnBeforeSerialize()
        {
            elementData = AllElements.ToArray();
        }

        public void OnAfterDeserialize()
        {
            foreach (GridElement element in elementData)
            {
                foreach (Vector2Int takenPosition in element.takenPositions)
                {
                    elements.Add(takenPosition, element);
                }
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks every cell within the given element's dimensions to find if it's possible to add to the given position.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="element">used to get search dimensions.</param>
        public virtual bool CanInsertAtPosition(Vector2Int position, GridElement element)
        {
            return CanInsertAtPosition(position, element.Size);
        }

        /// <summary>
        /// Checks every cell within the given dimensions to find if it's possible to add to the given position.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="dimensions">width and height of the area to check.</param>
        public virtual bool CanInsertAtPosition(Vector2Int position, Vector2Int dimensions)
        {
            for (int y = position.y; y < position.y + dimensions.y; y++)
            {
                for (int x = position.x; x < position.x + dimensions.x; x++)
                {
                    Vector2Int offsetPos = new (x, y);

                    if (ElementAtPosition(offsetPos)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Given the position and size, return an array of all the positions in that area.
        /// </summary>
        /// <param name="startPos">start position of the selection</param>
        /// <param name="dimensions">Height and Width of the area to select.</param>
        /// <returns></returns>
        public static Vector2Int[] SelectPositions(Vector2Int startPos, Vector2Int dimensions)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int y = startPos.y; y < startPos.y + dimensions.y; y++)
            {
                for (int x = startPos.x; x < startPos.x + dimensions.x; x++)
                {
                    Vector2Int newPos = new (x, y);
                    positions.Add(newPos);
                }
            }

            return positions.ToArray();
        }

        #endregion

        #endregion
    }

}