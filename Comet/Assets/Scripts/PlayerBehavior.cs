using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private int health;

    private int castingCrystals;

    private List<SpellCard> spellCards;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCastSpell(SpellCard card)
    {
        // Take passed cards relative range and apply it to
        // the players current position. set spaces in range to have
        // an effect which shows they are in range. 
        // players in those spaces are then made to take damage.
    }

    public void TakeDamage(int damage)
    {
        // pass damage value in and subtrack from player health
        // play any other effects like color flash or ragdoll activation.
    }

    public void Die()
    {
        // set health to zero and move player off screen.
    }
}
