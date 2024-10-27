using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    bool isOpen;

    Vector2Int cords;
    Tile occupiedTile;
    public List<Tile> neighbors = new List<Tile>();

    private void Start()
    {
        cords = GridManager.instance.GetCoordinatesFromPosition(transform.position);
        GetNeighbors(cords);
        isOpen = false;

        occupiedTile = GridManager.instance.GetTile(cords);

        occupiedTile.walkable = false;

        foreach (Tile neighbor in neighbors)
        {
            neighbor.nextToChest = true;
            neighbor.chest = this;
        }
    }

    public void OpenChest(PlayerBehavior player)
    {
        if (isOpen)
            return;

        isOpen = true;
        
        foreach (Tile neighbor in neighbors)
            neighbor.nextToChest = false;
        gameObject.SetActive(false);
        GridManager.instance.ClearTile(occupiedTile.cords);
        if (!player.photonView.IsMine)
            return;

        GameUI.instance.ThrowNotification("Chest Opened");
        player.getsNewCard = true;
    }


    void GetNeighbors(Vector2Int cords)
    {
        Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int direction in searchOrder)
        {
            Vector2Int neighborCords = cords + direction;
            if (GridManager.instance.Grid.ContainsKey(neighborCords))
                neighbors.Add(GridManager.instance.Grid[neighborCords]);
        }
    }
}
