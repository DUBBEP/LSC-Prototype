using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    bool isOpen;

    Vector2Int cords;
    Tile tile;
    public List<Tile> neighbors = new List<Tile>();


    private void Start()
    {
        GetNeighbors(cords);
        isOpen = false;

        tile = GridManager.instance.Grid[cords];

        tile.containsChest = true;
        tile.walkable = false;
        tile.chest = this;

        foreach (Tile neighbor in neighbors)
            neighbor.nextToChest = true;
    }


    public void OpenChest(PlayerBehavior player)
    {
        if (isOpen)
            return;

        foreach (Tile neighbor in neighbors)
            neighbor.nextToChest = false;
        gameObject.SetActive(false);

        if (!player.photonView.IsMine)
            return;
        
        SpellCard card = RandomCardGenerator.instance.GetRandomCard();
        HandManager.instance.AddCard(card.spellName);

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
