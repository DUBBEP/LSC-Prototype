using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpellCard", menuName = "SpellCard")]
public class SpellCard : ScriptableObject
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

    public enum rangeType
    {
        directionalLine,
        cross,
        star,

    }

    public string spellName;
    public string description;

    public int numberOfUses;
    public int power;
    public int castDelay;
    public int coolDown;
    public rarity cardRarity;
    public type cardType;
    
    public rangeType cardRangeType;

}   
