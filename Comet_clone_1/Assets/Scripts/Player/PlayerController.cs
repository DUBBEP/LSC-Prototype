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
                        Vector2Int cords = selectedTile.cords;

                        if (!selectedTile.walkable)
                            return;

                        if (!travelRange.Contains(selectedTile))
                            return;


                        photonView.RPC("RecordTargetCords", RpcTarget.All, cords.x, cords.y);

                        // Vector2 startCords = new Vector2Int((int)transform.position.x,
                        //     (int)transform.position.y) / gridManager.UnityGridSize;

                        playerIsMoving = false;
                        gridManager.SetAttackTileColor(travelRange, Color.white);
                        photonView.RPC("OnConfirmCast", RpcTarget.All, playerBehavior.id);
                    }
                }
            }
        }
    }


    #region Move Functions

    [PunRPC]
    public void OnMove(int id)
    {
        if (!photonView.IsMine)
        {
            PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();
            player.GenerateTravelRange(playerBehavior.MovementRange);
            player.myAction.effectRange = travelRange;
            player.myAction.card = spellRangeGenerator.CardLibrary["Move"];
            return;
        }

        if (playerIsMoving)
        {
            playerBehavior.cam.StopFollowing();
            playerIsMoving = false;
            gridManager.SetAttackTileColor(travelRange, Color.white);
            return;
        }

        if (preparingCast)
        {
            CancelCast();
            playerBehavior.cam.StartFollowing(this.transform);
        }

        playerIsMoving = true;
        GenerateTravelRange(playerBehavior.MovementRange);
        gridManager.SetMoveTileColors(travelRange);
        myAction.effectRange = travelRange;
        myAction.card = spellRangeGenerator.CardLibrary["Move"];
    }

    [PunRPC]
    void RecordTargetCords(int x, int y)
    {
        targetCords.x = x;
        targetCords.y = y;
    }

    [PunRPC]
    void MovePlayer()
    {
        transform.position = new Vector3(targetCords.x, transform.position.y, targetCords.y);
        playerBehavior.UpdateCords(targetCords);
    }

    void GenerateTravelRange(int range)
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
            gridManager.SetAttackTileColor(travelRange, Color.white);
        }

        preparingCast = true;
        GameUI.instance.SetHandUI(true);
    }

    [PunRPC]
    public void OnPrepareCast(int id, string cardName)
    {
        if (photonView.IsMine)
        {
            gridManager.SetAttackTileColor(myAction.effectRange, Color.white);
        }

        SpellCard card = spellRangeGenerator.CardLibrary[cardName];
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();


        player.myAction.effectRange = spellRangeGenerator.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords);
        player.myAction.card = card;

        if (photonView.IsMine)
        {
            GameUI.instance.SetConfirmCastButton(true);
            gridManager.SetAttackTileColor(myAction.effectRange, Color.red);
        }
    }

    [PunRPC]
    public void OnPrepareDirectionalCast(int id, string cardName, int dir)
    {
        if (photonView.IsMine)
        {
            gridManager.SetAttackTileColor(myAction.effectRange, Color.white);

        }

        Vector2Int direction = SetDirectionByInt(dir);
        SpellCard card = spellRangeGenerator.CardLibrary[cardName];
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();

        player.myAction.effectRange = spellRangeGenerator.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords, direction);
        player.myAction.card = card;

        if (photonView.IsMine)
        {
            GameUI.instance.SetConfirmCastButton(true);
            gridManager.SetAttackTileColor(myAction.effectRange, Color.red);
        }
    }
    [PunRPC]
    public void SetNewDirectionOfCast(int id, int dir)
    {
        if (photonView.IsMine)
            gridManager.SetAttackTileColor(myAction.effectRange, Color.white);

        Vector2Int direction = SetDirectionByInt(dir);

        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();

        player.myAction.effectRange = spellRangeGenerator.GenerateEffectRange(myAction.card.cardRangeType, playerBehavior.PlayerCords, direction);

        if (photonView.IsMine)
            gridManager.SetAttackTileColor(myAction.effectRange, Color.red);
    }


    [PunRPC]
    public void OnConfirmCast(int id)
    {
        Debug.Log("Sending Action to roundmanager");
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();

        RoundManager.instance.roundActions.Add(player.myAction);

        if (photonView.IsMine)
        {
            CancelCast();
            GameUI.instance.SetPlayerControls(false);
        }

        player.playerBehavior.turnCompleted = true;
        RoundManager.instance.CheckForUnreadyPlayers();
        // RoundManager.instance.DisplayAllActionInformation();
    }

    public void CancelCast()
    {
        GameUI.instance.SetConfirmCastButton(false);
        GameUI.instance.SetDirectionControls(false);
        GameUI.instance.SetHandUI(false);

        playerBehavior.cam.StopFollowing();

        if (myAction.card != null)
            gridManager.SetAttackTileColor(myAction.effectRange, Color.white);

        myAction.card = null;
        myAction.effectRange = null;
        directionalCast = false;
        preparingCast = false;
    }

    #endregion


    Vector2Int SetDirectionByInt(int dir)
    {

        switch (dir)
        {
            case 1:
                return Vector2Int.up;
            case 2:
                return Vector2Int.right;
            case 3:
                return Vector2Int.down;
            case 4:
                return Vector2Int.left;

        }
        return Vector2Int.right;
    }
}
