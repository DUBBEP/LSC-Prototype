using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PathFinding : MonoBehaviour
{
    [SerializeField] Vector2Int startCords;
    public Vector2Int StartCords { get { return startCords; } }

    [SerializeField] Vector2Int targetCords;
    public Vector2Int TargetCords { get { return targetCords; } }

    Tile startTile;
    Tile targetTile;
    Tile currentTile;

    Queue<Tile> frontier = new Queue<Tile>();
    Dictionary<Vector2Int, Tile> reached = new Dictionary<Vector2Int, Tile>();

    GridManager gridManager;
    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();

    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
            grid = gridManager.Grid;
    }
    
    // implementation is to be added


}
