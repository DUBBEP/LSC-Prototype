using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerController : MonoBehaviourPun
{
    bool playerIsMoving = false;
    bool directionalCast = false;
    bool preparingCast = false;
    
    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    List<Tile> travelRange = new List<Tile>();
    SpellCard selectedCard = null;
    public Action myAction = new Action(null, null, -1);
    public Vector2Int targetCords;

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

                        targetCords = hit.transform.GetComponent<Labeller>().cords;
                        // Vector2 startCords = new Vector2Int((int)transform.position.x,
                        //     (int)transform.position.y) / gridManager.UnityGridSize;

                        playerIsMoving = false;
                        gridManager.SetTileColor(travelRange, Color.white);
                        photonView.RPC("OnConfirmCast", RpcTarget.All, playerBehavior.id);
                    }
                }
            }
        }
    }


    #region Move Functions
    public void OnMove(SpellCard card)
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
        myAction.card = card;
        HighlightMovementRange(playerBehavior.MovementRange);
        myAction.effectRange = travelRange;
    }

    public void MovePlayer()
    {
        transform.position = new Vector3(targetCords.x, transform.position.y, targetCords.y);
        playerBehavior.UpdateCords(targetCords);
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
        myAction.card = selectedCard;
        gridManager.SetTileColor(myAction.effectRange, Color.red);
        GameUI.instance.SetConfirmCastButton(true);
    }
    public void OnPepareDirectionalCast(SpellCard card)
    {
        directionalCast = true;
        selectedCard = card;
        GameUI.instance.SetConfirmCastButton(true);
    }

    [PunRPC]
    public void OnConfirmCast(int id)
    {
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();
        RoundManager.instance.roundActions.Add(player.myAction);

        Debug.Log("Passed Id: " + id);

        Debug.Log("My Id:" + this.playerBehavior.id);

        if (id == this.playerBehavior.id)
        {
            Debug.Log("Clearing UI");
            CancelCast();
            TogglePlayerControls(false);
        }

        player.playerBehavior.turnCompleted = true;
        RoundManager.instance.CheckForUnreadyPlayers();
    }
    private void CancelCast()
    {
        GameUI.instance.SetConfirmCastButton(false);
        GameUI.instance.SetHandUI(false);

        if (selectedCard != null)
            gridManager.SetTileColor(myAction.effectRange, Color.white);

        myAction.card = null;
        myAction.effectRange = null;
        selectedCard = null;
        directionalCast = false;
        preparingCast = false;
    }

    #endregion



    public void TogglePlayerControls(bool toggle)
    {
        GameUI.instance.playerControls.SetActive(toggle);
    }
}
