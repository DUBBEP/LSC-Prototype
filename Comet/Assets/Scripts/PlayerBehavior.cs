using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Info")]
    public int id;
    private int curAttackerId;
    private Vector2Int playerCords;
    public Vector2Int PlayerCords { get { return playerCords; } }
    private List<SpellCard> spellCards;


    [Header("Stats")]
    private int castingCrystals;
    public int curHp;
    public int maxHp;
    public bool dead;
    [SerializeField]
    private int movementRange = 3;
    public int MovementRange { get { return movementRange; }}


    [Header("Components")]
    GridManager gridManager;
    public Player photonPlayer;


    /*
    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;


        // GameManager.instance.players[id - 1] = this;


        // is this not our local player?
        
        /*
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
        else
        {
            GameUI.instance.Initialize(this);
        }
        
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        playerCords = new Vector2Int(Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize), 
                                     Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize));
        gridManager.BlockTile(playerCords);

        Debug.Log("PlayerBehavior Active");
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

        curHp -= damage;

        if (curHp <= 0)
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
        curHp = 0;

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
