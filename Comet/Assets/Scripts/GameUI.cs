using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class GameUI : MonoBehaviour
{
    public GameObject spellHandUI;
    public GameObject confirmCast;
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
        healthText.text = player.curHp.ToString();
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
        playerController.OnMove();
    }

    public void OnCastButton()
    {
        playerController.OnCast();
    }

    public void OnConfirmCastButton()
    {
        playerController.OnConfirmCast();
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
