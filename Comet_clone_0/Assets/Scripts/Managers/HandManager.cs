using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField]
    private int cardLimit;

    public GameObject playerHandContainer;
    public GameObject cardDeck;
    public GameObject cardRemovalList;

    public GameObject cardRemovalscreenContainer;
    public List<GameObject> removeCardPool;


    public List<GameObject> playerHand;
    public List<GameObject> cardPool;


    public static HandManager instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        for (int i = 0; i < cardDeck.transform.childCount; i++)
            cardPool.Add(cardDeck.transform.GetChild(i).gameObject);

        for (int i = 0; i < playerHandContainer.transform.childCount; i++)
            playerHand.Add(playerHandContainer.transform.GetChild(i).gameObject);

        for (int i = 0; i < cardRemovalList.transform.childCount; i++)
            removeCardPool.Add(cardRemovalList.transform.GetChild(i).gameObject);
    }

    public int AddCard(string cardName)
    {
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
                card.gameObject.SetActive(false);
                return true;
            }
        }

        return false;
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
        Debug.Log("random int is: " + randomValue);
        Debug.Log("Random Card Picked:" + playerHand[randomValue].transform.GetChild(0).name);
        return playerHand[randomValue].transform.GetChild(0).name;
    }


    public void ForceCardRemoval(string newCard)
    {

    } 
}
