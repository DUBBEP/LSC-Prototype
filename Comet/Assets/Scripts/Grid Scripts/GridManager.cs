using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;
    public int UnityGridSize { get {  return unityGridSize; } }

    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> Grid { get { return grid; } }


    private void Awake()
    {
        CreateGrid();
    }

    public Tile GetTile (Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
            return grid[coordinates];

        return null;
    }

    public void BlockTile(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].walkable = false;
        }
    }

    public void resetNodes()
    {
        foreach (KeyValuePair<Vector2Int, Tile> entry in grid)
        {
            entry.Value.connectTo = null;
            entry.Value.explored = false;
            entry.Value.path = false;
        }
    }

    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        Vector2Int coordinates = new Vector2Int();

        coordinates.y = Mathf.RoundToInt(position.x / unityGridSize);
        coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);

        return coordinates;
    }

    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates)
    {
        Vector3 position = new Vector3();

        position.x = coordinates.x * unityGridSize;
        position.z = coordinates.y * unityGridSize;

        return position;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int cords = new Vector2Int(x, y);
                grid.Add(cords, new Tile(cords, true));
            }
        }
    }
}
