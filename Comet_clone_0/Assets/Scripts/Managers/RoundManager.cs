using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public struct Action
{
    public SpellCard card;
    public List<Tile> effectRange;
    public int playerId;

    public Action(SpellCard card, List<Tile> effectRange, int playerId)
    {
        this.card = card;
        this.effectRange = effectRange;
        this.playerId = playerId;
    }
};

public class RoundManager : MonoBehaviour
{
    public enum RoundState
    {
        roundStart,
        waitForPlayerActions,
        executePlayerActions,
        roundEnd
    }

    public List<Action> roundActions = new List<Action>();
    private List<int> interruptedPlayers = new List<int>();

    private RoundState state;
    public RoundState State { get { return state; } }


    public static RoundManager instance;

    private int readyPlayers;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpRound();
    }

    #region State Implementations
    void SetUpRound()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x == null)
                continue;

            if (x.dead)
            {
                Debug.Log(x.id + " is dead!");

                x.turnCompleted = true;

                if (x.photonView.IsMine)
                   GameUI.instance.SetPlayerControls(false);

                continue;
            }

            x.turnCompleted = false;

            if (x.photonView.IsMine)
                GameUI.instance.SetPlayerControls(true);
        }

        readyPlayers = 0;
        state = RoundState.waitForPlayerActions;
    }

    void WaitForPlayers()
    {
        return;
    }

    IEnumerator ExecuteActions(int waitAmmount)
    {
        Debug.Log("EXECUTE ACTIONS");

        roundActions = roundActions.OrderBy(x => x.card.castDelay).ToList();
        yield return new WaitForSeconds(waitAmmount);
        foreach (Action action in roundActions)
        {
            if (StopIfInterrupted(action))
                continue;

            // show effect range if one exists
            if (action.card.cardRangeType != SpellCard.rangeType.none)
                GridManager.instance.SetAttackTileColor(action.effectRange, Color.red);

            // focus camera player who's action this belongs to
            LookAtActivePlayer(action.playerId);

            Debug.Log("Waiting for a second");
            yield return new WaitForSecondsRealtime(waitAmmount);
            if (action.card.name == "MoveCard")
            {
                MovePlayer(action.playerId);
                LookAtActivePlayer(action.playerId);
                yield return new WaitForSecondsRealtime(waitAmmount);
                continue;
            }

            DamagePlayersInRange(action);
            GridManager.instance.SetAttackTileColor(action.effectRange, Color.white);
            yield return new WaitForSecondsRealtime(waitAmmount);
        }

        StopSpectating();
        state = RoundState.roundEnd;
        EndRound();
    }



    void EndRound()
    {
        roundActions.Clear();
        interruptedPlayers.Clear();
        state = RoundState.roundStart;
        SetUpRound();
    }
    #endregion

    void LookAtActivePlayer(int playerId)
    {
        foreach (PlayerBehavior player in GameManager.instance.players)
            if (player.photonView.IsMine)
                player.cam.StartFollowing(GameManager.instance.GetPlayer(playerId).transform);
    }
    void StopSpectating()
    {
        foreach (PlayerBehavior player in GameManager.instance.players)
            if (player.photonView.IsMine)
                player.cam.StopFollowing();
    }

    void MovePlayer(int playerId)
    {
        PlayerController player = GameManager.instance.GetPlayer(playerId).GetComponent<PlayerController>();
        player.photonView.RPC("MovePlayer", RpcTarget.All);

    }

    public void CheckForUnreadyPlayers()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x.turnCompleted == true || x.dead)
            {
                readyPlayers++;
            }
        }

        if (readyPlayers < GameManager.instance.players.Length)
        {
            readyPlayers = 0;
            return;
        }


        state = RoundState.executePlayerActions;
        StartCoroutine(ExecuteActions(1));
    }

    private void DamagePlayersInRange(Action action)
    {
        if (action.card.cardRangeType == SpellCard.rangeType.none)
            return;
        
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            if (player.dead)
                continue;

            Tile playerTile = GridManager.instance.Grid[player.PlayerCords];
            if (action.effectRange.Contains(playerTile))
            {
                Debug.Log("Player id: " + player.id + " taking damage");

                if (player.photonView.IsMine)
                    player.photonView.RPC("TakeDamage", player.photonPlayer, action.playerId, action.card.power);

                CheckForInterruptions(player.id);
            }
        }
    }

    private void CheckForInterruptions(int playerId)
    {
        foreach (Action action in roundActions)
        {
            if (playerId == action.playerId)
            {
                interruptedPlayers.Add(action.playerId);
                DisplayInterruption(action);

            }
        }
    }

    private bool StopIfInterrupted(Action action)
    {
        foreach (int i in interruptedPlayers)
            if (action.playerId == i)
                return true;

        return false;
    }

    private void DisplayInterruption(Action action)
    {
        Debug.Log("Action Interrupted");

        // display that the action was interrupted to screen
        // play any other effects related to an interrupted action
    }

    public void DisplayAllActionInformation()
    {
        foreach (Action c in roundActions)
        {
            Debug.Log("Round Action Player Id: " + c.playerId);
            Debug.Log("Round Action Effect Range Size: " + c.effectRange.Count);
            Debug.Log("Round Action card Name: " + c.card.name);
        }
    }


}
