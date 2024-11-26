using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Card Deck")]
    public GameObject cardDeck;
    public List<GameObject> cardPool;

    [Header("Card Removal")]
    public GameObject cardRemovalDeck;
    public List<GameObject> removeCardPool;
    public GameObject cardRemoveButtonsContainer;
    public GameObject newCardContainer;

    [Header("Player Hand")]
    public GameObject playerHandContainer;
    public List<GameObject> playerHand;
    
    [SerializeField]
    private int cardLimit;

    private string lastCardSelected;

    public static HandManager instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        for (int i = 0; i < cardDeck.transform.childCount; i++)
            cardPool.Add(cardDeck.transform.GetChild(i).gameObject);

        for (int i = 0; i < playerHandContainer.transform.childCount; i++)
            playerHand.Add(playerHandContainer.transform.GetChild(i).gameObject);

        for (int i = 0; i < cardRemovalDeck.transform.childCount; i++)
            removeCardPool.Add(cardRemovalDeck.transform.GetChild(i).gameObject);
    }

    public int AddCard(string cardName)
    {
        lastCardSelected = cardName;

        // if cards in hand meets or exceeds limit
        if (playerHand.Count >= cardLimit) return 2;


        // check if card is already in our hand
        foreach (GameObject card in playerHand)
        {
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
            {
                if (card.gameObject.activeSelf == false)
                {
                    card.gameObject.SetActive(true);
                    return 1;
                }
                else if (card.gameObject.activeSelf == true)
                {
                    GameUI.instance.ThrowNotification(cardName + " is alread held");
                    return 0;
                }
            }
        }

        // get card from card pool and add it to our hand
        foreach (GameObject card in cardPool)
        {
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
            {
                GameObject newCard = Instantiate(card, playerHandContainer.transform);
                playerHand.Add(newCard);
                CardUseTracker.instance.UpdateCardUseTracker(playerHand);
                GameUI.instance.ThrowNotification("Card Aquired: " + cardName);
                return 1;
            }
        }

        Debug.Log("Card does not exist");
        return 0;
    }

    public bool RemoveCard(string cardName)
    {
        foreach (GameObject card in playerHand)
        {
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
            {
                playerHand.Remove(card);
                Destroy(card);
                CardUseTracker.instance.UpdateCardUseTracker(playerHand);
                return true;
            }
        }
        return false;
    }

    public void SwapWithLastcardSelected(string oldCard)
    {
        RemoveCard(oldCard);
        AddCard(lastCardSelected);
    }

    public SpellCardDisplay GetCard(string cardName)
    {
        foreach (GameObject card in playerHand)
        {
            SpellCard spellCard = card.GetComponentInChildren<SpellCardDisplay>().spellCard;
            if (spellCard.spellName == cardName)
                return card.GetComponentInChildren<SpellCardDisplay>();
        }
        return null;
    }

    public string GetRandomCard()
    {
        int randomValue = Random.Range(0, playerHand.Count);
        return playerHand[randomValue].transform.GetChild(0).name;
    }

    public void UpdateCardRemovePanel(string newCard)
    {
        lastCardSelected = newCard;
        ClearRemovalButtons();
        //prepare card removal panel
        foreach (GameObject card in playerHand)
        {
            SpellCard cardInHand = card.GetComponentInChildren<SpellCardDisplay>().spellCard;
            foreach (GameObject cardRemoveButton in removeCardPool)
            {
                SpellCard removeButtonCounterpart = cardRemoveButton.GetComponentInChildren<SpellCardDisplay>().spellCard;

                if (cardInHand.spellName == removeButtonCounterpart.spellName)
                {
                    Instantiate(cardRemoveButton, cardRemoveButtonsContainer.transform);
                }
            }
        }

        foreach (GameObject cardRemoveButton in removeCardPool)
        {
            if (cardRemoveButton.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == newCard)
            {
                Instantiate(cardRemoveButton, newCardContainer.transform);
            }
        }
    }

    private void ClearRemovalButtons()
    {
        for (int i = 0; i < cardRemoveButtonsContainer.transform.childCount; i++)
        {
            Destroy(cardRemoveButtonsContainer.transform.GetChild(i).gameObject);
        }

        Destroy(newCardContainer.transform.GetChild(0).gameObject);
    }
}
