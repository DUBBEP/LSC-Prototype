using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections;

public class GameUI : MonoBehaviour
{
    public GameObject spellHandUI;
    public GameObject confirmCast;
    public GameObject playerControls;
    public GameObject directionControls;
    public GameObject notifications;

    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI castingCrystalText;
    public TextMeshProUGUI playerInfoText;

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
    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b>Kills:</b> " + player.kills;
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
