using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
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
        castingCrystalText.text = "<b>Crystals: </b>" + player.castingCrystals.ToString();
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
        playerController.photonView.RPC("OnMove", Photon.Pun.RpcTarget.All, player.id);
    }

    public void OnCastButton()
    {
        if (player.castingCrystals <= 0)
        {
            ThrowNotification("Unable to cast without casting crystals");
            return;
        }
        player.cam.StartFollowing(player.transform);
        playerController.OnCast();
    }

    public void OnConfirmCastButton()
    {
        playerController.photonView.RPC("OnConfirmCast", Photon.Pun.RpcTarget.All, player.id);
    }

    public void OnCardSelected(string cardName)
    {
        playerController.photonView.RPC("OnPrepareCast", Photon.Pun.RpcTarget.All, player.id, cardName);
        SetDirectionControls(false);
    }

    public void OnDirectionalCardSelected(string cardName)
    {
        playerController.photonView.RPC("OnPrepareDirectionalCast", Photon.Pun.RpcTarget.All, player.id, cardName, 2);
        SetDirectionControls(true);
    }

    public void OnDirectionSet(int dir)
    {
        playerController.photonView.RPC("SetNewDirectionOfCast", Photon.Pun.RpcTarget.All, player.id, dir);
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
            card.spellCard = RandomCardGenerator.instance.RollForCard(10).spellCard;
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

    public void SetHandUI(bool toggle)
    {
        spellHandUI.SetActive(toggle);
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
