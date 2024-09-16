using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private int health;
    private int castingCrystals;

    [SerializeField]
    private int movementRange = 3;
    public int MovementRange { get { return movementRange; }}

    private Vector2Int playerCords;
    public Vector2Int PlayerCords { get { return playerCords; }}

    private List<SpellCard> spellCards;

    GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        playerCords = new Vector2Int(Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize), 
                                     Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize));
        gridManager.BlockTile(playerCords);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCords(Vector2Int cords)
    {
        gridManager.ClearTile(playerCords);
        playerCords = cords;
        gridManager.BlockTile(playerCords);
    }

    public void PrepareCast(SpellCard card)
    {
        // This function will primarily serve to draw the spell range
        // so that the player may see what targets the spell will hit
        // before casting.
    }

    public void CastSpell(SpellCard card)
    {
        // This fuction is called when a player confirms a cast
        // on a spell that that they have already armed

        // This function will likely be queued by the turn manager and
        // executed in turn with the other players casts.

        // apply cards effect to the spaces within its effective range,
        // display spell visual effects. Check if 
    }


    // pass damage value in and subtrack from player health
    // play any other effects like color flash or ragdoll activation.
    public void TakeDamage(int damage)
    {

        health -= damage;

        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        Material mat = GetComponent<Material>();
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        mat.color = Color.white;
    }

    // set health to zero and move player off screen.
    public void Die()
    {
        health = 0;

        transform.position = new Vector3(1000f, 1000f, 1000f);
    }

    // adds selected spell card to cards this character holds
    public void AquireSpell(SpellCard card)
    {
        spellCards.Add(card);
    }


    // Removes specified card from held cards.
    public void RemoveSpell(SpellCard card)
    {
        foreach (SpellCard x in spellCards)
        {
            if (card.Equals(x))
                spellCards.Remove(x);
        }
    }

}
