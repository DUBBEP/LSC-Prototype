using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class Tile
{
    public Vector2Int cords;
    public bool walkable;
    public bool containsCrystal;
    public bool nextToChest;
    public bool explored;
    public bool path;
    public Tile connectTo;

    public CrystalBehavior crystal;
    public Chest chest;

    public Tile(Vector2Int Coordinates, bool walkable)
    {
        this.cords = Coordinates;
        this.walkable = walkable;
        this.containsCrystal = false;
        this.crystal = null;
    }
}
