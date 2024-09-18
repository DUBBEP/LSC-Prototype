using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerController : MonoBehaviour
{
    // [SerializeField] float movementSpeed = 1f;

    bool playerIsMoving = false;
    bool directionalCast = false;

    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    List<Tile> travelRange = new List<Tile>();
    GridManager gridManager;
    PlayerBehavior playerBehavior;
    SpellRangeGenerator spellRangeGenerator;
    GameUI gameUI;

    SpellCard selectedCard = null;


    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        gameUI = FindObjectOfType<GameUI>();
        playerBehavior = GetComponent<PlayerBehavior>();
        spellRangeGenerator = FindObjectOfType<SpellRangeGenerator>();
    }

    private void Update()
    {
        if (directionalCast)
        {
            // track mouse position here
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

                        gridManager.SetTileColor(travelRange, Color.white);
                        playerIsMoving = false;
                    }
                }
            }
        }
    }

    
    public void OnMoveButton()
    {
        if (playerIsMoving)
        {
            playerIsMoving = false;
            gridManager.SetTileColor(travelRange, Color.white);
            return;
        }

        if (selectedCard != null)
            CancelCast();

        playerIsMoving = true;
        HighlightMovementRange(playerBehavior.MovementRange);
    }

    public void OnCastButton()
    {
        // display the Players list of cards to the screen
        if (selectedCard != null)
        {
            CancelCast();
            return;
        }

        if (playerIsMoving)
        {
            playerIsMoving = false;
            gridManager.SetTileColor(travelRange, Color.white);
        }


        gameUI.ToggleHandUI();
    }

    private void CancelCast()
    {
        gameUI.SetConfirmCastButton(false);
        gameUI.ToggleHandUI();
        gridManager.SetTileColor(spellRangeGenerator.GenerateEffectRange(
                selectedCard.cardRangeType, playerBehavior.PlayerCords), Color.white);
        selectedCard = null;
        directionalCast = false;
    }

    public void OnConfirmCast()
    {
        SendCastToQueue();
        CancelCast();
    }

    public void OnPrepareCast(SpellCard card)
    {
        selectedCard = card;
        gridManager.SetTileColor(spellRangeGenerator.GenerateEffectRange(
            card.cardRangeType, playerBehavior.PlayerCords), Color.red);
        gameUI.SetConfirmCastButton(true);
    }
    public void OnPepareDirectionalCast(SpellCard card)
    {
        directionalCast = true;
        selectedCard = card;
        gameUI.SetConfirmCastButton(true);
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


            // This loop does not lead to the intended effect and must be changed
            foreach (Tile y in travelRange)
            {
                if (exploreRange.Contains(y))
                    exploreRange.Remove(y);
                else
                    exploreRange.Add(y);
            }
        }

        gridManager.SetMoveTileColors(travelRange);

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

    public void SendCastToQueue()
    {
        return;
    }
}
