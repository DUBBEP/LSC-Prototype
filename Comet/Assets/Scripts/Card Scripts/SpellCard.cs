using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpellCard", menuName = "SpellCard")]
public abstract class SpellCard : ScriptableObject
{
    public enum rarity
    {
        common,
        rare,
        epic,
        legendary
    }

    public enum type
    {
        basic,
        special
    }

    public string spellName;
    public string description;

    public int numberOfUses;
    public int power;
    public int castDelay;
    public int coolDown;
    public rarity cardRarity;
    public type cardType;

    public List<Tile> BuildCardRange(Vector2Int cords)
    {
        // This function will take the given coordinates to build
        // out a set spell effect range dependent on the current players positon.

        return null;
    }
}   
