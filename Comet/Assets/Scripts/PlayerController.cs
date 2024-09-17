using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerController : MonoBehaviour
{
    // [SerializeField] float movementSpeed = 1f;

    bool playerIsMoving = false;
    bool preparingCast = false;

    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    List<Tile> travelRange = new List<Tile>();
    GridManager gridManager;
    PlayerBehavior playerBehavior;
    SpellCard selectedCard;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerBehavior = GetComponent<PlayerBehavior>();
    }

    private void Update()
    {
        if (preparingCast)
        {
            playerBehavior.PrepareCast(selectedCard);
            // if a player is preparing a cast then keep track of their mouse position
            // if a spell has directional aim then check if the mouse is in the direction
            // that they want to cast relative to the player
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                if(hit.transform.tag == "Tile")
                {
                    if (playerIsMoving)
                    {
                        Tile selectedTile = gridManager.Grid[hit.transform.gameObject.GetComponent<Labeller>().cords];

                        if (!selectedTile.walkable)
                            return;

                        if (!travelRange.Contains(selectedTile))
                            return;

                        Vector2Int targetCords = hit.transform.GetComponent<Labeller>().cords;
                        Vector2 startCords = new Vector2Int((int)transform.position.x,
                            (int)transform.position.y) / gridManager.UnityGridSize;

                        transform.position = new Vector3(targetCords.x, transform.position.y, targetCords.y);
                        playerBehavior.UpdateCords(targetCords);

                        gridManager.revertTileColors(travelRange);
                        playerIsMoving = false;
                    }
                }
            }
        }
    }

    
    public void OnMove()
    {
        if (playerIsMoving)
            return;

        playerIsMoving = true;
        HighlightMovementRange(playerBehavior.MovementRange);
    }


    public void OnPrepareCast(SpellCard card)
    {
        preparingCast = true;
        selectedCard = card;
    }


    void HighlightMovementRange(int range)
    {
        // take the passed range value to build the characters movement range
        // start with the players current position and check for available spots
        // in the cardinal directions. For any available spaces found, add them to
        // the list of the spaces in the players current movement range.
        // then check the cardinal directions of the newly added spaces and add tiles
        // next to those ones. Repeat this process for however many times the passed range
        // value indicates.

        List<Tile> exploreRange = new List<Tile>();

        travelRange.Clear();
        travelRange.Add(gridManager.Grid[playerBehavior.PlayerCords]);
        exploreRange.Add(gridManager.Grid[playerBehavior.PlayerCords]);

        // Get every tile in player movement radius
        for (int i = 0; i < playerBehavior.MovementRange; ++i)
        {

            foreach (Tile x in exploreRange)
            {
                ExploreNeighbors(x);
            }

            foreach (Tile y in travelRange)
            {
                if (exploreRange.Contains(y))
                    exploreRange.Remove(y);
                else
                    exploreRange.Add(y);
            }
        }

        gridManager.SetTileColors(travelRange);

    }



    void ExploreNeighbors(Tile tile)
    {
        foreach (Vector2Int direction in searchOrder)
        {
            Vector2Int neighborCords = tile.cords + direction;
            if (gridManager.Grid.ContainsKey(neighborCords))
            {
                if (!travelRange.Contains(gridManager.Grid[neighborCords]))
                {
                    travelRange.Add(gridManager.Grid[neighborCords]);
                }
            }
        }
    }
}
