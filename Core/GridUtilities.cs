using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hitbox.GridFramework.Utilities
{
    /// <summary>
    /// Utilities for the Hitbox Grid System.
    /// </summary>
    public static class GridUtilities
    {
        /// <summary>
        /// Get the decimal average position of the given positions, useful for setting object positions.
        /// </summary>
        /// <param name="positions">the collection of positions to average.</param>
        /// <param name="offset">applied to both axes of the result.</param>
        /// <returns></returns>
        public static Vector2 GetAveragePosition(IReadOnlyCollection<Vector2Int> positions, float offset = 0f)
        {
            Vector2 sum = positions.Aggregate(Vector2.zero,
                (current, slot) => current + slot);

            return (sum / positions.Count) + new Vector2(offset, offset);
        }
        
        /// <summary>
        /// Get the centre of the given dimensions, whilst attempting to keep the grid as central as possible to the
        /// given difference, differences larger than the central point will prefer right, whilst smaller values
        /// will go left. This is equivalent to GridCentre when dealing with odd numbers since they can be
        /// centred without any decimal.
        /// </summary>
        /// <param name="difference">difference of position in relation to central point (assumed 0)</param>
        /// <param name="size">dimensions of the area to get centre from</param>
        /// <returns></returns>
        public static Vector2Int SmartGridCentre(Vector2 difference, Vector2Int size)
        {
            int xOffset;
            int yOffset;

            if (size.x % 2 == 0)
            {
                xOffset = difference.x < 0
                    ? Mathf.CeilToInt(GridCentre(size).x)
                    : Mathf.FloorToInt(GridCentre(size).x);
            }
            else
            {
                xOffset = Mathf.CeilToInt(GridCentre(size).x);
            }
            
            if (size.y % 2 == 0)
            {
                yOffset = difference.y > 0
                    ? Mathf.FloorToInt(GridCentre(size).y)
                    : Mathf.CeilToInt(GridCentre(size).y);
            }
            else
            {
                yOffset = Mathf.CeilToInt(GridCentre(size).y);
            }

            
            return new Vector2Int(xOffset, yOffset);
        }

        /// <summary>
        /// Get the centre of the given dimensions, whilst attempting to keep the grid as central as possible to the
        /// given difference, differences larger than the central point will prefer right, whilst smaller values
        /// will go left. This is equivalent to GridCentre when dealing with odd numbers since they can be
        /// centred without any decimal.
        /// </summary>
        /// <param name="difference">difference of position in relation to central point (assumed 0)</param>
        /// <param name="centre"></param>
        /// <param name="size">dimensions of the area to get centre from</param>
        /// <returns></returns>
        public static Vector2Int SmartGridCentre(Vector2 difference, Vector2 centre, Vector2Int size)
        {
            int xOffset;
            int yOffset;

            if (size.x % 2 == 0)
            {
                xOffset = difference.x < centre.x
                    ? Mathf.CeilToInt(GridCentre(size).x)
                    : Mathf.FloorToInt(GridCentre(size).x);
            }
            else
            {
                xOffset = Mathf.CeilToInt(GridCentre(size).x);
            }
            
            if (size.y % 2 == 0)
            {
                yOffset = difference.y > centre.y
                    ? Mathf.FloorToInt(GridCentre(size).y)
                    : Mathf.CeilToInt(GridCentre(size).y);
            }
            else
            {
                yOffset = Mathf.CeilToInt(GridCentre(size).y);
            }

            
            return new Vector2Int(xOffset, yOffset);
        }

        /// <summary>
        /// Get the center of the given dimensions rounded to grid position.
        /// </summary>
        /// <param name="size">dimensions of the item to centre.</param>
        /// <returns></returns>
        public static Vector2 GridCentre(Vector2Int size) => new()
        {
            x = size.x / 2 - .5f,
            y = size.y / 2 - .5f
        };
    }
}