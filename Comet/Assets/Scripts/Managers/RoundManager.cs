using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Queue<Action> roundActions = new Queue<Action>();

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

    #region State Implementation
    void SetUpRound()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            x.turnCompleted = false;
        }

        state = RoundState.waitForPlayerActions;
    }

    void WaitForPlayers()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x.turnCompleted == false)
                return;
        }

        state = RoundState.executePlayerActions;
    }

    void ExecuteActions()
    {

    }

    void EndRound()
    {
        roundActions.Clear();
    }
    #endregion

}
