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
    public bool interrupted;

    public Action(SpellCard card, List<Tile> effectRange, int playerId)
    {
        this.card = card;
        this.effectRange = effectRange;
        this.playerId = playerId;
        this.interrupted = false;
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
            x.GetComponent<PlayerController>().myAction.interrupted = false;

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

    IEnumerator ExecuteActions()
    {
        Debug.Log("EXECUTE ACTIONS");

        roundActions = roundActions.OrderBy(x => x.card.castDelay).ToList();

        foreach (Action action in roundActions)
        {
            if (action.interrupted) continue;



            Debug.Log("Waiting for a second");
            yield return new WaitForSecondsRealtime(1);
            if (action.card.name == "MoveCard")
                MovePlayer(action.playerId);
            else
                DamagePlayersInRange(action);
        }

        state = RoundState.roundEnd;
        EndRound();
    }

    void EndRound()
    {
        roundActions.Clear();
        state = RoundState.roundStart;
        SetUpRound();
    }
    #endregion


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
        StartCoroutine(ExecuteActions());
    }

    private void DamagePlayersInRange(Action action)
    {
        if (action.card.cardRangeType == SpellCard.rangeType.none)
            return;
        
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            if (player.dead)
                continue;

            if (!player.photonView.IsMine)
                continue;

            Tile playerTile = GridManager.instance.Grid[player.PlayerCords];
            if (action.effectRange.Contains(playerTile))
            {
                Debug.Log("Player id: " + player.id + " taking damage");
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
                InterruptAction(action);
        }
    }

    private void InterruptAction(Action action)
    {
        action.interrupted = true;

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
