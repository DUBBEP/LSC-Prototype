using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;

    public Vector2Int GridSize { get { return gridSize; } }
    public int UnityGridSize { get {  return unityGridSize; } }

    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> Grid { get { return grid; } }

    public static GridManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            gameObject.SetActive(false);

        CreateGrid();

    }

    public Tile GetTile (Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
            return grid[coordinates];

        return null;
    }

    public void SetMoveTileColors(List<Tile> tiles)
    {
        foreach (Tile x in tiles)
        {
            MeshRenderer tileMaterial = GameObject.Find(x.cords.ToString()).GetComponentInChildren<MeshRenderer>();

            if (x.walkable)
                tileMaterial.material.color = Color.green;
            else
                tileMaterial.material.color = Color.red;

            if (x.containsCrystal)
                tileMaterial.material.color = Color.cyan;

            if (x.nextToChest)
                tileMaterial.material.color = Color.yellow;
        }
    }
    public void SetTileColor(List<Tile> tiles, Color color)
    {
        if (tiles == null)
            return;

        foreach (Tile x in tiles)
        {
            MeshRenderer tileMaterial = GameObject.Find(x.cords.ToString()).GetComponentInChildren<MeshRenderer>();
            tileMaterial.material.color = color;
        }
    }

    public void BlockTile(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].walkable = false;
        }
    }
    public void ClearTile(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].walkable = true;
        }
    }
    public void CollectCrystal(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].containsCrystal = false;
        }
    }

    public void PlaceCrystal(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].containsCrystal = true;
        }
    }
    public void resetTiles()
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

        coordinates.x = Mathf.RoundToInt(position.x / unityGridSize);
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

                if (GameObject.Find(cords.ToString()))
                    grid.Add(cords, new Tile(cords, true));
            }
        }
    }
}
