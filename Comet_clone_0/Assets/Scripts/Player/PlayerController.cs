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
    
    public List<Tile> travelRange = new List<Tile>();
    public Action myAction = new Action(null, null, -1, Vector2Int.zero);
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

                        playerIsMoving = false;
                        gridManager.SetTileColor(travelRange, Color.white);
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
            if (myAction.card != null && myAction.card.spellName == "Teleport")
            {
                player.travelRange = myAction.effectRange;
            }
            else
            {
                player.travelRange = SpellRangeGenerator.instance.GenerateEffectRange(playerBehavior.PlayerCords, playerBehavior.MovementRange);
                player.myAction.effectRange = travelRange;
                player.myAction.card = SpellRangeGenerator.instance.CardLibrary["Move"];
            }

            return;
        }

        if (playerIsMoving)
        {
            playerBehavior.cam.RestAtPlayerQuadrant();
            playerIsMoving = false;
            gridManager.SetTileColor(travelRange, Color.white);
            return;
        }

        if (preparingCast)
        {
            CancelCast();
            playerBehavior.cam.StartFollowing(this.transform);
        }
        playerIsMoving = true;

        if (myAction.card != null && myAction.card.spellName == "Teleport")
        {
            travelRange = myAction.effectRange;
        }
        else
        {
            travelRange = SpellRangeGenerator.instance.GenerateEffectRange(playerBehavior.PlayerCords, playerBehavior.MovementRange);
            myAction.effectRange = travelRange;
            myAction.card = SpellRangeGenerator.instance.CardLibrary["Move"];
        }
        gridManager.SetMoveTileColors(travelRange);
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
        GameUI.instance.SetHandUI(true);
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

        if (photonView.IsMine)
            GameUI.instance.OnSetFollowCam();

        preparingCast = true;
    }

    [PunRPC]
    public void OnPrepareCast(int id, string cardName)
    {
        SpellCard card = SpellRangeGenerator.instance.CardLibrary[cardName];
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();

        player.myAction.effectRange = SpellRangeGenerator.instance.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords);
        player.myAction.card = card;
        player.myAction.direction = Vector2Int.zero;

        if (photonView.IsMine)
        {
            gridManager.SetTileColor(myAction.effectRange, Color.white);

            if (card.spellName != "Teleport")
                gridManager.SetTileColor(myAction.effectRange, Color.red);
            else
                GameUI.instance.OnSetFreeCam();

            GameUI.instance.SetConfirmCastButton(true);
        }

        if (card.spellName == "Teleport")
            OnMove(id);

    }

    [PunRPC]
    public void OnPrepareDirectionalCast(int id, string cardName, int dir)
    {
        SetDirectionOfCast(id, dir, cardName);
        if (photonView.IsMine)
            GameUI.instance.SetConfirmCastButton(true);
    }

    [PunRPC]
    public void SetDirectionOfCast(int id, int dir, string cardName)
    {
        if (photonView.IsMine)
            gridManager.SetTileColor(myAction.effectRange, Color.white);

        Vector2Int direction = SetDirectionByInt(dir);
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();
        SpellCard card;

        if (cardName == "none")
            card = player.myAction.card;
        else
            card = SpellRangeGenerator.instance.CardLibrary[cardName];

        player.myAction.effectRange = SpellRangeGenerator.instance.GenerateEffectRange(card.cardRangeType, playerBehavior.PlayerCords, direction);
        player.myAction.card = card;
        player.myAction.direction = direction;

        if (photonView.IsMine)
            gridManager.SetTileColor(myAction.effectRange, Color.red);
    }


    [PunRPC]
    public void OnConfirmCast(int id)
    {
        PlayerController player = GameManager.instance.GetPlayer(id).GetComponent<PlayerController>();
        RoundManager.instance.roundActions.Add(player.myAction);

        if (photonView.IsMine)
        {
            if (myAction.card.cardActionType != SpellCard.actionType.none && myAction.card.spellName != "Move")
                CardUseTracker.instance.UseCard(myAction.card);
            CancelCast();
            GameUI.instance.SetPlayerControls(false);
            GameUI.instance.SetWaitingPanel(true);
        }

        player.playerBehavior.turnCompleted = true;
        RoundManager.instance.CheckForUnreadyPlayers();
    }

    public void CancelCast()
    {
        GameUI.instance.SetConfirmCastButton(false);
        GameUI.instance.SetDirectionControls(false);
        GameUI.instance.SetHandUI(false);

        playerBehavior.cam.RestAtPlayerQuadrant();

        if (myAction.card != null)
            gridManager.SetTileColor(myAction.effectRange, Color.white);

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
