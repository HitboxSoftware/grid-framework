using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.GridFramework
{
    /// <summary>
    /// Contains all of the data needed for elements, inherit from this class to add additional data and
    /// logic to elements.
    /// </summary>
    public abstract class GridElementProfile : ScriptableObject
    {
        #region --- VARIABLES ---

        /// <summary> Size of the element within a grid. </summary>
        [Header("Element Properties")] 
        public Vector2Int size = Vector2Int.one;
        
        // --- EVENTS ---
        /// <summary>
        /// Invoked whenever an element has been updated.
        /// </summary>
        public event System.Action OnUpdate;

        #endregion
        
        #region --- METHODS ---

        /// <summary>
        /// Creates a new runtime data object, override this when implementing custom runtime data.
        /// </summary>
        public virtual GridElementRuntime GetRuntime => new ();
        
        /// <summary>
        /// Attempts to combine two elements together allowing for logic such as element stacking.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="placedElement"></param>
        /// <returns></returns>
        public virtual (GridElement, GridElement) TryCombineElements(GridElement target, GridElement placedElement) { return (target, placedElement); }

        /// <summary>
        /// This is called whenever an element has been updated so that the frontend can be notified.
        /// </summary>
        public void Updated()
        {
            OnUpdate?.Invoke();
        }
        
        #endregion
    }

    /// <summary>
    /// Contains all of the runtime data for an element, excluding position.
    /// <para/>
    /// Inherit from this to add additional runtime data to elements.
    /// Examples of this could be container runtime that contains grid data of the container.
    /// </summary>
    
    [System.Serializable]
    public class GridElementRuntime
    {
        
    }
}