using System.Linq;
using UnityEngine;
using Hitbox.GridFramework;
using Grid = Hitbox.GridFramework.Grid;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class GridElement : ISerializationCallbackReceiver
{
    #region --- VARIABLES ---

    /// <summary>Information about the grid element such as name and size.</summary>
    public GridElementProfile profile;
    public GridElementRuntime elementRuntime;

    /// <summary>
    /// Returns the size of this element, can be overriden to add additional behaviour (i.e. rotation).
    /// </summary>
    public virtual Vector2Int Size => profile.size;

    /// <summary>
    /// The positions in the grid that this element takes up.
    /// First index is used as the origin position for the element in the grid.
    /// <para/>
    /// When moving a grid element (not dropping or deleting), this should be updated <i>after</i> the element
    /// has been added to the new position, since this is used to return the item to its original position if
    /// it's not placed successfully.
    /// </summary>
    [System.NonSerialized] public Vector2Int[] takenPositions;
    [SerializeField] private Vector2Int elementPos;

    private Grid parentGrid;
    
    /// <summary>
    /// Grid runtime data created when generating the item in the grid.
    /// Generally should be non-serialized, unless only one type of data is used. 
    /// </summary>
    [System.NonSerialized] public GridElementRuntime runtime;
    private string jsonRuntimeData; // JSON Serialized Runtime Data
    private string runtimeDataType;
    
    #endregion
    
    #region --- METHODS ---

    #region Grid

    /// <summary>
    /// Checks if the element is contained in the given grid.
    /// </summary>
    public virtual bool InGrid(Grid grid) => parentGrid == grid;
    
    /// <summary>
    /// Set the element's parent to the given grid.
    /// </summary>
    public virtual void SetGrid(Grid grid) => parentGrid = grid;

    /// <summary>
    /// Checks if the element is contained in the given position.
    /// </summary>
    public virtual bool InPosition(Vector2Int position) => takenPositions.Contains(position);
    
    #endregion
    
    #region Serialization Callbacks

    public virtual void OnBeforeSerialize()
    {
        if (runtime == null) return;
        jsonRuntimeData = JsonUtility.ToJson(runtime); 
        runtimeDataType = runtime.GetType().ToString(); // Used to get object type when deserializing data.
        elementPos = takenPositions[0];
    }

    public virtual void OnAfterDeserialize()
    {
        var type = System.Type.GetType(runtimeDataType);
        runtime = (GridElementRuntime)JsonUtility.FromJson(jsonRuntimeData, type);
        takenPositions[0] = elementPos;
    }

    #endregion

    #region Constructors

    public GridElement(GridElementProfile profile, Grid parentGrid, Vector2Int[] takenPositions = null, GridElementRuntime gridElementRuntime = null)
    {
        this.profile = profile;
        this.takenPositions = takenPositions;
        this.parentGrid = parentGrid;
            
        runtime = gridElementRuntime ?? this.profile.GetRuntime;
    }

    #endregion

    #endregion
}
