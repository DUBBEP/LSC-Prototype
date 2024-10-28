using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurnOrderUI : MonoBehaviour
{
    [SerializeField]
    private float lerpSmoothFactor;
    [SerializeField]
    Vector2 restingPosition;
    [SerializeField]
    private List<GameObject> playerList = new List<GameObject>();

    public RectTransform rectTransform;

    private bool returnToRest;
    public void DisplayTurnOrder(List<Action> actionOrder)
    {
        DisableListItems();
        DisplayPanelInCenter();


        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            Action playerAction = actionOrder[i];

            if (RoundManager.instance.CheckIfInterrupted(playerAction))
                continue;

            string actionName;

            if (playerAction.card.cardActionType == SpellCard.actionType.move)
                actionName = "Move";
            else
                actionName = "Cast Spell";

            playerList[i].SetActive(true);
            playerList[i].transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text
                = GameManager.instance.GetPlayer(playerAction.playerId).photonPlayer.NickName;
            playerList[i].transform.Find("ActionNameText").GetComponent<TextMeshProUGUI>().text = actionName;
            playerList[i].transform.Find("SpeedText").GetComponent<TextMeshProUGUI>().text = playerAction.card.castDelay.ToString();
        }
    }

    void DisableListItems()
    {
        foreach (GameObject item in playerList)
            item.SetActive(false);
    }

    void Update()
    {
        if (returnToRest)
        {
            Vector3 newPos = Vector2.Lerp(rectTransform.anchoredPosition, restingPosition, lerpSmoothFactor);
            Vector3 newScale = Vector3.Lerp(rectTransform.localScale, Vector3.one * 0.5f, lerpSmoothFactor);
            
            rectTransform.anchoredPosition = newPos;
            rectTransform.localScale = newScale;

            if ((rectTransform.anchoredPosition - restingPosition).magnitude < 0.05f)
                returnToRest = false;
        }
    }


    void DisplayPanelInCenter()
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one * 0.8f;
    }

    public void ReturnToRestPosition()
    {
        returnToRest = true;
    }
}
