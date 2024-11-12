using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class GameUI : MonoBehaviourPun
{
    public GameObject spellHandUI;
    public GameObject confirmCast;
    public GameObject playerControls;
    public GameObject directionControls;
    public GameObject notifications;
    public GameObject waitingPanel;
    public GameObject cardSelectPanel;
    public GameObject followCamButton;
    public GameObject freeCamButton;
    public GameObject freeCamControls;


    public Transform cardSelectContainer;

    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI castingCrystalText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI unreadyPlayerList;

    public Image winBackGround;
    public TextMeshProUGUI winText;

    private PlayerBehavior player;
    private PlayerController playerController;


    public TurnOrderUI turnOrderUI;

    public static GameUI instance;



    private void Awake()
    {
        instance = this;
    }

    public void UpdateHealthText()
    {
        healthText.text = "<b>Health: </b>" + player.curHp.ToString();
    }
    public void UpdateCastingCrystalText()
    {
        castingCrystalText.text = "<b>Crystals: </b>" + player.curCastingCrystals.ToString();
    }

    public void Initialize(PlayerBehavior localPlayer)
    {
        player = localPlayer;
        playerController = localPlayer.GetComponent<PlayerController>();
    }

    public void SetWinText(string winnerName)
    {
        winBackGround.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }

    public void OnMoveButton()
    {
        player.cam.StartFollowing(player.transform);
        playerController.photonView.RPC("OnMove", RpcTarget.All, player.id);
    }

    public void OnCastButton()
    {
        if (player.curCastingCrystals <= 0)
        {
            ThrowNotification("Unable to cast without casting crystals");
            return;
        }
        player.cam.StartFollowing(player.transform);
        playerController.OnCast();
    }

    public void OnConfirmCastButton()
    {
        playerController.photonView.RPC("OnConfirmCast", RpcTarget.All, player.id);
    }

    public void OnCardSelected(string cardName)
    {
        playerController.photonView.RPC("OnPrepareCast", RpcTarget.All, player.id, cardName);
        SetDirectionControls(false);
    }

    public void OnDirectionalCardSelected(string cardName)
    {
        playerController.photonView.RPC("OnPrepareDirectionalCast", RpcTarget.All, player.id, cardName, 2);
        SetDirectionControls(true);
    }

    public void OnDirectionSet(int dir)
    {
        playerController.photonView.RPC("SetDirectionOfCast", RpcTarget.All, player.id, dir, "none");
    }

    public void SetCardSelectPanel(bool toggle)
    {
        cardSelectPanel.SetActive(toggle);
    }

    public void GetCardSelectCards()
    {
        List<SpellCardDisplay> cards = new List<SpellCardDisplay>();

        for (int i = 0; i < cardSelectContainer.transform.childCount; i++)
            cards.Add(cardSelectContainer.transform.GetChild(i).GetComponentInChildren<SpellCardDisplay>());

        foreach (SpellCardDisplay card in cards)
        {
            card.spellCard = RandomCardGenerator.instance.RollForCard(10).spellCard;
            card.UpdateCardDisplay();
        }
    }

    public void OnNewCardSelected(SpellCardDisplay card)
    {
        HandManager.instance.AddCard(card.spellCard.spellName);
        SetCardSelectPanel(false);
    }

    public void OnSetFreeCam()
    {
        player.cam.SetCamMode(CameraBehavior.cameraMode.freeCam);

        freeCamButton.SetActive(false);
        followCamButton.SetActive(true);
        freeCamControls.SetActive(true);

        ThrowNotification("Camera Mode: Free Cam");
    }

    public void OnSetFollowCam()
    {
        player.cam.SetCamMode(CameraBehavior.cameraMode.followCam);

        freeCamButton.SetActive(true);
        followCamButton.SetActive(false);
        freeCamControls.SetActive(false);

        ThrowNotification("Camera Mode: Follow Cam");
    }

    public void SetTimerText(bool toggle)
    {
        timerText.gameObject.SetActive(toggle);
    }

    [PunRPC]
    public void UpdateTimerText(int timeInt)
    {
        timerText.text = "<b>Time Left: </b>" + timeInt.ToString();
    }

    public void SetWaitingPanel(bool toggle)
    {
        waitingPanel.gameObject.SetActive(toggle);
    }

    public void UpdateUnreadyPlayerList()
    {
        unreadyPlayerList.text = string.Empty;
        foreach (PlayerBehavior player in GameManager.instance.players)
        {
            if (player.turnCompleted == false && !player.dead)
            {
                unreadyPlayerList.text = unreadyPlayerList.text + "Waiting for " +
                    player.photonPlayer.NickName + "...\n";
            }
        }
    }

    public void ToggleHandUI()
    {
        spellHandUI.SetActive(!spellHandUI.activeSelf);
    }

    public void SetConfirmCastButton(bool toggle)
    {
        confirmCast.SetActive(toggle);
    }

    public void SetPlayerControls(bool toggle)
    {
        playerControls.SetActive(toggle);
    }

    public void SetDirectionControls(bool toggle)
    {
        directionControls.SetActive(toggle);
    }

    public void ThrowNotification(string message)
    {
        StartCoroutine(SetNotificationText(message));
    }

    public IEnumerator SetNotificationText(string message)
    {
        notifications.SetActive(true);
        notificationText.text = message;
        yield return new WaitForSeconds(4);
        notificationText.text = "";
        notifications.SetActive(false);
    }


}
