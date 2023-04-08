using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.GridFramework.Grids
{
    public class SizedGrid : Grid
    {
        #region --- VARIABLES ---

        public Vector2Int size;

        #endregion

        #region --- METHODS ---

        /// <summary>
        /// Checks every cell within the given dimensions to find if it's possible to add to the given position.
        /// Also ensures that the area is within the grid's boundaries.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="dimensions">width and height of the area to check.</param>
        public override bool CanInsertAtPosition(Vector2Int position, Vector2Int dimensions)
        {
            // Ensure Area is within world size.
            if (position.x + dimensions.x - 1 >= size.x || // Check X Boundary
                position.y + dimensions.y - 1 >= size.y || // Check Y Boundary
                position.x < 0 || position.y < 0) return false; // Check [< 0] XY Boundaries.
        
            return base.CanInsertAtPosition(position, dimensions);
        }
    
        #endregion

        #region --- CONSTRUCTOR ---

        public SizedGrid(Vector2Int size)
        {
            this.size = size;
        }

        #endregion
    }

}