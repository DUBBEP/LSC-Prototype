using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCardGenerator : MonoBehaviour
{
    public GameObject cardDeck;
    public List<SpellCardDisplay> cardPool;

    public static RandomCardGenerator instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        for (int i = 0; i < cardDeck.transform.childCount; i++)
            cardPool.Add(cardDeck.transform.GetChild(i).GetComponentInChildren<SpellCardDisplay>());
    }

    public SpellCardDisplay RollForCard(int numOfTimes)
    {
        SpellCardDisplay card = GetRandomCard();
        for (int i = 0; i < numOfTimes; i++)
        {
            SpellCard pulledSpellCard = card.spellCard;
            foreach (GameObject cardObject in HandManager.instance.playerHand)
            {
                SpellCard cardInHand = cardObject.GetComponentInChildren<SpellCardDisplay>().spellCard;

                Debug.Log("CardRolled: " + pulledSpellCard.spellName);


                if (cardInHand.spellName == pulledSpellCard.spellName)
                    card = null;
            }

            if (card != null)
            {
                Debug.Log("Roll attempts before giving card: " + i);
                return card;
            }
        }
        return GetRandomCard();
    }

    SpellCardDisplay GetRandomCard()
    {
        int randomValue = Random.Range(0, 101);
        SpellCard.rarity rarityPull;

        Debug.Log("Value Rolled: " + randomValue);

        if (randomValue < 40)
            rarityPull = SpellCard.rarity.common;
        else if (randomValue < 70)
            rarityPull = SpellCard.rarity.rare;
        else if (randomValue < 85)
            rarityPull = SpellCard.rarity.epic;
        else
            rarityPull = SpellCard.rarity.legendary;

        List<SpellCardDisplay> pullPool = GetCardsOfRarity(rarityPull);

        randomValue = Random.Range(0, pullPool.Count);
        return pullPool[randomValue];   
    }

    List<SpellCardDisplay> GetCardsOfRarity(SpellCard.rarity rarity)
    {
        List<SpellCardDisplay> cards = new List<SpellCardDisplay>();
        foreach (SpellCardDisplay card in cardPool)
            if (card.spellCard.cardRarity == rarity)
                cards.Add(card);

        return cards;
    }

}
