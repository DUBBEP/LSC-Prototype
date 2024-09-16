using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class Tile
{
    public Vector2Int cords;
    public bool walkable;
    public bool explored;
    public bool path;
    public Tile connectTo;

    public Tile(Vector2Int Coordinates, bool walkable)
    {
        this.cords = Coordinates;
        this.walkable = walkable;
    }
}
