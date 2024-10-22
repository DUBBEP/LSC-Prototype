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

    [SerializeField]
    public float roundPaceTime;
    [SerializeField]
    public int roundPlanningTime;

    private float roundTimer;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (roundTimer > 0 && state == RoundState.waitForPlayerActions)
        {
            roundTimer -= Time.deltaTime;
            GameUI.instance.UpdateTimerText((int)roundTimer);
        }
        else if (roundTimer <= 0 && state == RoundState.waitForPlayerActions)
            if (PhotonNetwork.IsMasterClient)
                ForceEndTurn();
    }

    #region State Implementations
    public void SetUpRound()
    {
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (PhotonNetwork.IsMasterClient)
                GameManager.instance.CheckWinCondition();

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

            if (x.photonView.IsMine && !x.isStunned && !x.isConfused)
            {
                GameUI.instance.SetPlayerControls(true);
            }

            x.PrepForNewRound();


        }

        CheckForCardsAquired();
        GameUI.instance.SetTimerText(true);
        roundTimer = roundPlanningTime;
        state = RoundState.waitForPlayerActions;
    }

    IEnumerator ExecuteActions(float waitAmmount)
    {
        GameUI.instance.SetWaitingPanel(false);
        GameUI.instance.SetTimerText(false);

        Debug.Log("EXECUTE ACTIONS");

        roundActions = roundActions.OrderBy(x => x.card.castDelay).ToList();
        yield return new WaitForSeconds(waitAmmount);
        foreach (Action action in roundActions)
        {
            // show effect range if one exists
            if (action.card.cardRangeType != SpellCard.rangeType.none)
                GridManager.instance.SetAttackTileColor(action.effectRange, Color.red);

            // focus camera player who's action this belongs to
            LookAtActivePlayer(action.playerId);

            if (CheckIfInterrupted(action))
            {
                if (action.card.cardActionType == SpellCard.actionType.none)
                    GameUI.instance.ThrowNotification(GameManager.instance.GetPlayer(action.playerId).photonPlayer.NickName + " does not act");
                else
                    GameUI.instance.ThrowNotification(GameManager.instance.GetPlayer(action.playerId).photonPlayer.NickName + " has been interrupted");
                yield return new WaitForSecondsRealtime(waitAmmount);
                continue;
            }
            else if (action.card.cardActionType == SpellCard.actionType.move)
                GameUI.instance.ThrowNotification(GameManager.instance.GetPlayer(action.playerId).photonPlayer.NickName + " moves");
            else
                GameUI.instance.ThrowNotification(GameManager.instance.GetPlayer(action.playerId).photonPlayer.NickName + " casts " + action.card.spellName);

            Debug.Log("Waiting for a second");
            yield return new WaitForSecondsRealtime(waitAmmount);

            ExecuteActionEffect(action);
            yield return new WaitForSecondsRealtime(waitAmmount);
        }

        StopSpectating();
        state = RoundState.roundEnd;
        EndRound();
    }

    void ExecuteActionEffect(Action action)
    {
        PlayerBehavior player = GameManager.instance.GetPlayer(action.playerId);

        if (action.card.cardActionType == SpellCard.actionType.move)
        {
            MovePlayer(action.playerId);
            LookAtActivePlayer(action.playerId);
        }
        else if (action.card.cardActionType == SpellCard.actionType.mirror)
        {
            player.photonView.RPC("ActivateMirror", player.photonPlayer);
        }
        else if (action.card.cardActionType == SpellCard.actionType.heal)
        {
            player.photonView.RPC("Heal", player.photonPlayer, action.card.power);
        }
        else
        {
            EffectPlayersInRange(action);
            GridManager.instance.SetAttackTileColor(action.effectRange, Color.white);
        }


        if (action.card.name != "MoveCard")
        {
            --player.castingCrystals;
            if (player.photonView.IsMine)
                GameUI.instance.UpdateCastingCrystalText();
        }
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
    public void StopSpectating()
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

    private void EffectPlayersInRange(Action action)
    {
        if (action.card.cardRangeType == SpellCard.rangeType.none)
            return;

        PlayerBehavior actingPlayer = GameManager.instance.GetPlayer(action.playerId);

        foreach (PlayerBehavior effectedPlayer in GameManager.instance.players)
        {
            if (effectedPlayer.dead)
                continue;

            Tile playerTile = GridManager.instance.Grid[effectedPlayer.PlayerCords];
            if (action.effectRange.Contains(playerTile))
            {
                Debug.Log("Player id: " + effectedPlayer.id + " taking damage");

                if (effectedPlayer.mirrorActive)
                {
                    GameUI.instance.ThrowNotification(effectedPlayer.photonPlayer.NickName + " reflects " + actingPlayer.photonPlayer.NickName + "'s attack!");

                    if (actingPlayer.photonView.IsMine)
                        CallEffectFunctions(action, effectedPlayer, actingPlayer);
                }
                else if (effectedPlayer.photonView.IsMine)
                    CallEffectFunctions(action, actingPlayer, effectedPlayer);

                CheckForInterruptions(effectedPlayer.id, action.card.castDelay);
            }
        }
    }

    private static void CallEffectFunctions(Action action, PlayerBehavior actingPlayer, PlayerBehavior effectedPlayer)
    {
        switch (action.card.cardActionType)
        {
            case (SpellCard.actionType.normalDamage):
                effectedPlayer.photonView.RPC("TakeDamage", effectedPlayer.photonPlayer, actingPlayer.id, action.card.power);
                break;
            case (SpellCard.actionType.confusion):
                if (action.card.power > 0)
                    effectedPlayer.photonView.RPC("TakeDamage", effectedPlayer.photonPlayer, actingPlayer.id, action.card.power);
                effectedPlayer.photonView.RPC("BecomeConfused", effectedPlayer.photonPlayer, actingPlayer.id);
                break;
            case (SpellCard.actionType.stun):
                effectedPlayer.photonView.RPC("BecomeStunned", effectedPlayer.photonPlayer, actingPlayer.id);
                break;

        }
    }

    void CheckForCardsAquired()
    {
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            if (player == null) continue;

            if (player.photonView.IsMine && player.getsNewCard)
            {
                GameUI.instance.SetCardSelectPanel(true);
                GameUI.instance.GetCardSelectCards();
                player.getsNewCard = false;
            }
        }
    }

    private void CheckForInterruptions(int playerId, int spellDelay)
    {
        foreach (Action action in roundActions)
        {
            if (action.card.castDelay <= spellDelay)
                continue;

            if (playerId == action.playerId)
            {
                interruptedPlayers.Add(action.playerId);
                DisplayInterruption(action);
            }
        }
    }

    private bool CheckIfInterrupted(Action action)
    {
        foreach (int i in interruptedPlayers)
            if (action.playerId == i)
                return true;

        return false;
    }

    public void CheckForUnreadyPlayers()
    {
        readyPlayers = 0;
        GameUI.instance.UpdateUnreadyPlayerList();
        foreach (PlayerBehavior x in GameManager.instance.players)
        {
            if (x.turnCompleted == true || x.dead)
            {
                readyPlayers++;
            }
        }

        if (readyPlayers < GameManager.instance.players.Length)
        {
            return;
        }


        state = RoundState.executePlayerActions;
        StartCoroutine(ExecuteActions(roundPaceTime));
    }

    public void ForceEndTurn()
    {
        roundTimer = 0;
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            if (PhotonNetwork.IsMasterClient && player.turnCompleted == false && !player.dead)
            {
                interruptedPlayers.Add(player.id);
                player.photonView.RPC("OnPrepareCast", RpcTarget.All, player.id, "EmptyCard");
                player.photonView.RPC("OnConfirmCast", RpcTarget.All, player.id);
            }
        }
        GameUI.instance.ThrowNotification("One or more players have failed to act.");
    }

    private void DisplayInterruption(Action action)
    {
        Debug.Log("Action Interrupted");

        PlayerBehavior player = GameManager.instance.GetPlayer(action.playerId);

        GameUI.instance.ThrowNotification(player.photonPlayer.NickName + " has been interrupted");
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
