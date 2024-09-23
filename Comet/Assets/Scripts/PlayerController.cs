using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerController : MonoBehaviour
{
    bool playerIsMoving = false;
    bool directionalCast = false;
    bool preparingCast = false;
    
    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    List<Tile> travelRange = new List<Tile>();
    SpellCard selectedCard = null;
    Action myAction = new Action(null, null, -1);

    GridManager gridManager;
    PlayerBehavior playerBehavior;
    SpellRangeGenerator spellRangeGenerator;




    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerBehavior = GetComponent<PlayerBehavior>();
        spellRangeGenerator = FindObjectOfType<SpellRangeGenerator>();

        myAction.playerId = playerBehavior.id;
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

    #region Move Functions
    public void OnMove()
    {
        if (playerIsMoving)
        {
            playerIsMoving = false;
            gridManager.SetTileColor(travelRange, Color.white);
            return;
        }

        if (preparingCast)
            CancelCast();

            playerIsMoving = true;
        HighlightMovementRange(playerBehavior.MovementRange);
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

    #endregion

    #region Cast Functions
    public void OnCast()
    {
        // display the Players list of cards to the screen
        if (preparingCast)
        {
            CancelCast();
            return;
        }

        if (playerIsMoving)
        {
            playerIsMoving = false;
            gridManager.SetTileColor(travelRange, Color.white);
        }

        preparingCast = true;
        GameUI.instance.SetHandUI(true);
    }

    public void OnPrepareCast(SpellCard card)
    {
        selectedCard = card;
        myAction.effectRange = spellRangeGenerator.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords);
        gridManager.SetTileColor(myAction.effectRange, Color.red);
        GameUI.instance.SetConfirmCastButton(true);
    }
    public void OnPepareDirectionalCast(SpellCard card)
    {
        directionalCast = true;
        selectedCard = card;
        GameUI.instance.SetConfirmCastButton(true);
    }

    public void OnConfirmCast()
    {
        RoundManager.instance.roundActions.Enqueue(myAction);
        CancelCast();
        playerBehavior.turnCompleted = true;
    }
    private void CancelCast()
    {
        GameUI.instance.SetConfirmCastButton(false);
        GameUI.instance.SetHandUI(false);

        if (selectedCard != null)
            gridManager.SetTileColor(spellRangeGenerator.GenerateEffectRange(
                selectedCard.cardRangeType, playerBehavior.PlayerCords), Color.white);

        selectedCard = null;
        directionalCast = false;
        preparingCast = false;
    }

    #endregion



}
