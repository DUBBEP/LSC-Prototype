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
        none,
        move,
        directionalLine,
        cross,
        star,
        laser,
        hand,
        slice,
        flashbang,
        orb

    }

    public enum actionType
    {
        none,
        move,
        normalDamage,
        mirror,
        stun,
        confusion,
        heal
    }


    public string spellName;
    public string description;

    public int numberOfUses;
    public int power;
    public int castDelay;
    public int coolDown;
    public rarity cardRarity;
    public type cardType;

    public bool rangeIsDirectional;
    public rangeType cardRangeType;
    public actionType cardActionType;

    public SpellVisualEffect visualEffect;
}
