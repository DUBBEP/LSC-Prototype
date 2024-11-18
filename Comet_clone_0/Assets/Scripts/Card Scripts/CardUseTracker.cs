using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUseTracker : MonoBehaviour
{
    Dictionary<SpellCard, int> cardUseTracker = new Dictionary<SpellCard, int>();
    public static CardUseTracker instance;

    private void Awake() { instance = this; }

    private void Start() { UpdateCardUseTracker(HandManager.instance.playerHand); }

    public void UpdateCardUseTracker(List<GameObject> cardList)
    {
        foreach (GameObject card in cardList)
        {
            SpellCard spellCard = card.GetComponentInChildren<SpellCardDisplay>().spellCard;
            if (!cardUseTracker.ContainsKey(spellCard))
            {
                Debug.Log("Adding " + spellCard.spellName + " to card use tracker");
                cardUseTracker.Add(spellCard, spellCard.numberOfUses);
            }
        }
    }

    public void UseCard(SpellCard card)
    {
        --cardUseTracker[card];
        if (cardUseTracker[card] <= 0)
            RemoveCard(card);
        else
            UpdateCardDisplay(card);
    }

    private void UpdateCardDisplay(SpellCard card)
    {
        SpellCardDisplay cardDisplay = HandManager.instance.GetCard(card.spellName);
        if (cardDisplay != null)
            cardDisplay.useCountText.text = cardUseTracker[card].ToString();
    }

    private void RemoveCard(SpellCard card)
    {
        HandManager.instance.RemoveCard(card.spellName);
        cardUseTracker.Remove(card);
        GameUI.instance.ThrowNotification(card.spellName + " Has run out of uses");
    }
}
