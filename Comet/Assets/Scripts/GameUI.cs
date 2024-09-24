using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class GameUI : MonoBehaviour
{
    public GameObject spellHandUI;
    public GameObject confirmCast;
    public GameObject playerControls;

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

    public void UpdateHealthBar()
    {
        healthText.text = "<b>Health: </b>" + player.curHp.ToString();
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

    public void OnMoveButton(SpellCard card)
    {
        playerController.OnMove(card);
    }

    public void OnCastButton()
    {
        playerController.OnCast();
    }

    public void OnConfirmCastButton()
    {
        playerController.photonView.RPC("OnConfirmCast", Photon.Pun.RpcTarget.All, player.id);
        playerController.CancelCast();
        playerController.TogglePlayerControls(false);
    }


    public void OnCardSelected(SpellCard card)
    {
        playerController.OnPrepareCast(card);
    }

    public void OnDirectionalCardSelected(SpellCard card)
    {
        playerController.OnPepareDirectionalCast(card);
    }


    public void SetHandUI(bool toggle)
    {
        spellHandUI.SetActive(toggle);
    }

    public void SetConfirmCastButton(bool toggle)
    {
        confirmCast.SetActive(toggle);
    }
}
