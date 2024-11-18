using System.Collections.Generic;
using System.Linq;
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
        actionOrder = actionOrder.OrderBy(x => x.card.castDelay).ToList();
        DisableListItems();
        DisplayPanelInCenter();

        int skippedPlayers = 0;
        int currentPlayerIndex = 0;

        for (int i = 0; i < RoundManager.instance.roundActions.Count; i++)
        {
            Action playerAction = actionOrder[i];

            if (RoundManager.instance.CheckIfInterrupted(playerAction))
            {
                ++skippedPlayers;
                continue;
            }

            string actionName = (playerAction.card.cardActionType == SpellCard.actionType.move) ? "Move" : "Cast Spell";

            currentPlayerIndex = i - skippedPlayers;

            playerList[currentPlayerIndex].SetActive(true);
            playerList[currentPlayerIndex].transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text
                = GameManager.instance.GetPlayer(playerAction.playerId).photonPlayer.NickName;
            playerList[currentPlayerIndex].transform.Find("ActionNameText").GetComponent<TextMeshProUGUI>().text = actionName;
            playerList[currentPlayerIndex].transform.Find("SpeedText").GetComponent<TextMeshProUGUI>().text = playerAction.card.castDelay.ToString();
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
            Vector3 newPos = Vector3.Lerp(rectTransform.anchoredPosition, restingPosition, Time.deltaTime * lerpSmoothFactor);
            Vector3 newScale = Vector3.Lerp(rectTransform.localScale, Vector3.one * 0.5f, Time.deltaTime * lerpSmoothFactor);
            
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
