using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject playerHandContainer;
    public GameObject cardDeck;

    private List<GameObject> playerHand;
    private List<GameObject> cardPool;


    public static HandManager instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        foreach (Transform child in cardDeck.transform)
            cardPool.Add(child.gameObject);

        foreach (Transform child in playerHandContainer.transform)
            playerHand.Add(child.gameObject);
    }

    public bool AddCard(string cardName)
    {
        // check if card is already in our hand
        foreach (GameObject card in playerHand)
        {
            if (card.GetComponentInChildren<SpellCard>().spellName == cardName)
            {
                if (card.gameObject.activeSelf == false)
                {
                    card.gameObject.SetActive(true);
                    return true;
                }
                else if (card.gameObject.activeSelf == true)
                {
                    GameUI.instance.ThrowNotification("Card is alread held");
                    return false;
                }
            }
        }

        // get card from card pool and add it to our hand
        foreach (GameObject card in cardPool)
        {
            if (card.GetComponentInChildren<SpellCard>().spellName == cardName)
            {
                GameObject newCard = Instantiate(card, playerHandContainer.transform);
                playerHand.Add(newCard);
                return true;
            }
        }

        Debug.Log("Card does not exist");
        return false;
    }

    public bool RemoveCard(string cardName)
    {
        foreach (GameObject card in playerHand)
        {
            if (card.GetComponentInChildren<SpellCard>().spellName == cardName)
            {
                card.gameObject.SetActive(false);
                return true;
            }
        }

        return false;
    }

}
