using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerController : MonoBehaviourPun
{
    bool playerIsMoving = false;
    bool preparingCast = false;
    
    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    public List<Tile> travelRange = new List<Tile>();
    public Action myAction = new Action(null, null, -1);
    public Vector2Int targetCords;

    GridManager gridManager;
    PlayerBehavior playerBehavior;




    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerBehavior = GetComponent<PlayerBehavior>();
    }

    private void Update()
    {
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
            player.travelRange = SpellRangeGenerator.instance.GenerateEffectRange(player.playerBehavior.PlayerCords, player.playerBehavior.MovementRange);
            player.myAction.effectRange = travelRange;
            player.myAction.card = SpellRangeGenerator.instance.CardLibrary["Move"];
            return;
        }

        if (playerIsMoving)
        {
            playerBehavior.cam.RestAtPlayerQuadrant();
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
        travelRange = SpellRangeGenerator.instance.GenerateEffectRange(playerBehavior.PlayerCords, playerBehavior.MovementRange);
        gridManager.SetMoveTileColors(travelRange);
        myAction.effectRange = travelRange;
        myAction.card = SpellRangeGenerator.instance.CardLibrary["Move"];
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

        Tile tile = GridManager.instance.GetTile(targetCords);

        if (tile == null)
            return;

        if (tile.nextToChest)
            tile.chest.OpenChest(playerBehavior);

        if (tile.containsCrystal)
            tile.crystal.Collect(playerBehavior);    
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

        SpellCard card = SpellRangeGenerator.instance.CardLibrary[cardName];
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();


        player.myAction.effectRange = SpellRangeGenerator.instance.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords);
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
        SpellCard card = SpellRangeGenerator.instance.CardLibrary[cardName];
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();

        player.myAction.effectRange = SpellRangeGenerator.instance.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords, direction);
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

        player.myAction.effectRange = SpellRangeGenerator.instance.GenerateEffectRange(myAction.card.cardRangeType, playerBehavior.PlayerCords, direction);

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
            GameUI.instance.SetWaitingPanel(true);
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

        playerBehavior.cam.RestAtPlayerQuadrant();

        if (myAction.card != null)
            gridManager.SetAttackTileColor(myAction.effectRange, Color.white);

        myAction.card = null;
        myAction.effectRange = null;
        preparingCast = false;
    }

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

    #endregion



}
