using System.Collections;
using System.Collections.Generic;
using Hitbox.GridFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Grid = Hitbox.GridFramework.Grid;

[System.Serializable]
public struct GridData
{
    public GridElementData[] contents;

    public GridData(GridElementData[] contents)
    {
        this.contents = contents;
    }

    /// <summary>
    /// Creates a new grid and loads the given data.
    /// </summary>
    /// <param name="data">The data to load into a new grid.</param>
    /// <param name="callback">This is called once the grid has finished loading.</param>
    public static IEnumerator LoadFromData(GridData data, System.Action<Grid> callback)
    {
        var grid = new Grid();

        foreach (var elementData in data.contents)
        {
            AsyncOperationHandle<GridElementAddressableProfile> handle =
                Addressables.LoadAssetAsync<GridElementAddressableProfile>(elementData.profileReference);

            yield return handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded) yield break;

            GridElement element = new (handle.Result, grid)
            {
                runtime = elementData.runtimeData
            };

            grid.InsertElementAtPosition(element, elementData.pos);
        }

        callback(grid);
    }

    /// <summary>
    /// Create a serializable object from the given grid, this will only save elements with profiles implementing "GridElementAddressableProfile"
    /// </summary>
    /// <param name="grid">grid to save data from</param>
    /// <returns></returns>
    public static GridData SaveToData(Grid grid)
    {
        if (grid == null) return new GridData();

        List<GridElementData> data = new ();
        foreach (var element in grid.AllElements)
        {
            if (element.profile is not GridElementAddressableProfile addressableProfile) continue;

            data.Add(new GridElementData(
                profileReference: addressableProfile.reference.AssetGUID, 
                runtimeData: element.runtime, 
                pos: element.takenPositions[0]));
        }

        return new GridData(data.ToArray());
    }
}

[System.Serializable]
public struct GridElementData
{
    public string profileReference;
    public GridElementRuntime runtimeData;
    public Vector2Int pos;

    public GridElementData(string profileReference, GridElementRuntime runtimeData, Vector2Int pos)
    {
        this.profileReference = profileReference;
        this.runtimeData = runtimeData;
        this.pos = pos;
    }
}
