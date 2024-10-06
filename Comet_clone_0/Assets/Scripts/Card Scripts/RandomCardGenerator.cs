using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCardGenerator : MonoBehaviour
{
    public GameObject cardDeck;
    public List<SpellCard> cardPool;

    public static RandomCardGenerator instance;

    private void Awake() { instance = this; }

    private void Start()
    {
        foreach (Transform child in cardDeck.transform)
            cardPool.Add(child.GetComponentInChildren<SpellCard>());
    }

    public SpellCard GetRandomCard()
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

        List<SpellCard> pullPool = GetCardsOfRarity(rarityPull);

        randomValue = Random.Range(0, pullPool.Count);
        return pullPool[randomValue];   
    }

    List<SpellCard> GetCardsOfRarity(SpellCard.rarity rarity)
    {
        List<SpellCard> cards = new List<SpellCard>();
        foreach (SpellCard card in cardPool)
            if (card.cardRarity == rarity)
                cards.Add(card);

        return cards;
    }

}
