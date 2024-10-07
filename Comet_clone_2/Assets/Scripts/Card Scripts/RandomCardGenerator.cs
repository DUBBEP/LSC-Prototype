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

    public SpellCardDisplay GetRandomCard()
    {
        int randomValue = Random.Range(0, 101);
        SpellCard.rarity rarityPull;

        if (randomValue < 50)
            rarityPull = SpellCard.rarity.common;
        else if (randomValue < 80)
            rarityPull = SpellCard.rarity.rare;
        else if (randomValue < 90)
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
