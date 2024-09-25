using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

    private RoundState state;
    public RoundState State { get { return state; } }


    public static RoundManager instance;




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
        state = RoundState.roundStart;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case RoundState.roundStart:
                SetUpRound();
                break;
            case RoundState.waitForPlayerActions:
                WaitForPlayers();
                break;
            case RoundState.executePlayerActions:
                ExecuteActions();
                break;
            case RoundState.roundEnd:
                EndRound();
                break;
        }
    }

    #region State Implementations
    void SetUpRound()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x == null)
                return;

            x.turnCompleted = false;
            x.GetComponent<PlayerController>().TogglePlayerControls(true);
        }

        state = RoundState.waitForPlayerActions;
    }

    void WaitForPlayers()
    {
        return;
    }

    void ExecuteActions()
    {
        Debug.Log("EXECUTING ACTIONS");
        foreach (Action c in roundActions)
            Debug.Log("roundAction #1: " + c.card.castDelay.ToString());

        roundActions = roundActions.OrderBy(x => x.card.castDelay).ToList();

        foreach (Action c in roundActions)
            Debug.Log("roundAction delay #1: " + c.card.castDelay.ToString());

        foreach (Action action in roundActions)
        {
            if (action.card.name == "MoveCard")
                GameManager.instance.GetPlayer(action.playerId).Move();

            DamagePlayersInRange(action);
        }

        state = RoundState.roundEnd;
    }


    void EndRound()
    {
        roundActions.Clear();
        state = RoundState.roundStart;
    }
    #endregion


    public void CheckForUnreadyPlayers()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x.turnCompleted == false)
                return;
        }

        state = RoundState.executePlayerActions;
    }

    private void DamagePlayersInRange(Action action)
    {
        if (action.card.cardRangeType == SpellCard.rangeType.none)
            return;
        
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            Debug.Log("Player id: " + player.id + "Being compared for damage");
            Tile playerTile = GridManager.instance.Grid[player.PlayerCords];
            if (action.effectRange.Contains(playerTile))
            {
                Debug.Log("Player id: " + player.id + " taking damage");
                player.photonView.RPC("TakeDamage", player.photonPlayer, action.playerId, action.card.power);
            }
        }
    }
}
