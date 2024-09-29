using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviourPun
{
    [Header("Info")]
    public int id;
    private int curAttackerId;
    private Vector2Int playerCords;
    public Vector2Int PlayerCords { get { return playerCords; } }
    private List<SpellCard> spellCards;
    private bool flashingDamage;
    public bool turnCompleted = false;



    [Header("Stats")]
    public int castingCrystals;
    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead ;
    [SerializeField]
    private int movementRange = 3;
    public int MovementRange { get { return movementRange; }}


    [Header("Components")]
    GridManager gridManager;
    public Player photonPlayer;
    public MeshRenderer mr;
    PlayerController playerController;



    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        playerController = GetComponent<PlayerController>();
        playerController.myAction.playerId = id;

        GameManager.instance.players[id - 1] = this;


        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            GameUI.instance.Initialize(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        playerCords = new Vector2Int(Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize), 
                                     Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize));
        gridManager.BlockTile(playerCords);

        GameUI.instance.UpdateHealthText();
        GameUI.instance.UpdateCastingCrystalText();
        GameUI.instance.UpdatePlayerInfoText();

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

    // pass damage value in and subtrack from player health
    // play any other effects like color flash or ragdoll activation.
    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (dead)
            return;

        curHp -= damage;
        curAttackerId = attackerId;

        // flash the player red
        photonView.RPC("DamageFlash", RpcTarget.All);
        // update the health bar UI
        GameUI.instance.UpdateHealthText();
        // die if no health left
        if (curHp <= 0)
            photonView.RPC("Die", RpcTarget.All);

    }

    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;

        StartCoroutine(DamageFlashCoRoutine());

        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;

            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            Debug.Log("Flash on");


            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;

            Debug.Log("Flash Off");

        }
    }

    // set health to zero and move player off screen.
    [PunRPC]
    public void Die()
    {
        curHp = 0;
        dead = true;
        GameManager.instance.alivePlayers--;

        playerController.photonView.RPC("RecordTargetCords", RpcTarget.All, -100, -100);
        playerController.photonView.RPC("MovePlayer", RpcTarget.All);


        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        

        if (photonView.IsMine)
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
        }

    // adds selected spell card to cards this character holds
    public void AquireSpell(SpellCard card)
    {
        spellCards.Add(card);
    }

    [PunRPC]
    public void AddKill()
    {
        kills++;

        // update the UI
        GameUI.instance.UpdatePlayerInfoText();
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
