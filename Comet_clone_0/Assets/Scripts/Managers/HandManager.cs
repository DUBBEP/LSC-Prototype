using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject playerHandContainer;
    public GameObject cardDeck;

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
    }

    public bool AddCard(string cardName)
    {
        // check if card is already in our hand
        foreach (GameObject card in playerHand)
        {
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
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
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
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
            if (card.GetComponentInChildren<SpellCardDisplay>().spellCard.spellName == cardName)
            {
                card.gameObject.SetActive(false);
                return true;
            }
        }

        return false;
    }

}
