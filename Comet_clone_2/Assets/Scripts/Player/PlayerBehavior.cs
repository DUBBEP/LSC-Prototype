using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class PlayerBehavior : MonoBehaviourPun
{
    [Header("Info")]
    public int id;
    private int curAttackerId;
    private Vector2Int playerCords;
    public Vector2Int PlayerCords { get { return playerCords; } }
    private bool flashingDamage;
    public bool turnCompleted = false;
    public bool isConfused = false;
    public bool mirrorActive = false;
    public bool isStunned = false;
    public bool getsNewCard = false;



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
    public Player photonPlayer;
    public MeshRenderer mr;
    private PlayerController playerController;
    public CameraBehavior cam;
    public HeaderInfo headerInfo;
    public GameObject mirrorVisual;
    public Animator playerModelAnimator;
    public Animator playerAttackAnimator;


    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        castingCrystals = 3;

        playerController = GetComponent<PlayerController>();
        playerController.myAction.playerId = id;

        headerInfo.Initialize(player.NickName, maxHp);

        GameManager.instance.players[id - 1] = this;


        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            GameUI.instance.Initialize(this);
<<<<<<< HEAD
            cam.Initialize(this);
            cam.transform.parent = null;
            cam.transform.position = GameManager.instance.camDefaultPos;

=======
            cam.transform.parent = null;
            cam.transform.position = GameManager.instance.camDefaultPos;
>>>>>>> parent of 3c48487 (Removed Clones)
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCords = new Vector2Int(Mathf.RoundToInt(transform.position.x / GridManager.instance.UnityGridSize), 
                                     Mathf.RoundToInt(transform.position.z / GridManager.instance.UnityGridSize));
        GridManager.instance.BlockTile(playerCords);

        GameUI.instance.UpdateHealthText();
        GameUI.instance.UpdateCastingCrystalText();
    }

    public void UpdateCords(Vector2Int cords)
    {
        GridManager.instance.ClearTile(playerCords);
        playerCords = cords;
        GridManager.instance.BlockTile(playerCords);
    }

    [PunRPC]
    public void BecomeStunned(int attackerId)
    {
        curAttackerId = attackerId;

        GameUI.instance.ThrowNotification("You have become stunned");

        photonView.RPC("SetStunned", RpcTarget.All);
    }

    [PunRPC]
    void SetStunned()
    {
        isStunned = true;
    }

    [PunRPC]
    public void BecomeConfused(int attackerId)
    {
        curAttackerId = attackerId;

        if (photonView.IsMine)
            GameUI.instance.ThrowNotification("You have become Confused");
        
        photonView.RPC("SetConfused", RpcTarget.All);
    }

    [PunRPC]
    void SetConfused()
    {
        isConfused = true;
    }

    [PunRPC]
    public void ActivateMirror()
    {
        mirrorActive = true;
        photonView.RPC("ToggleMirror", RpcTarget.All, true);
    }

    [PunRPC]
    public void ToggleMirror(bool toggle)
    {
        mirrorActive = toggle;
        mirrorVisual.SetActive(toggle);
    }

    [PunRPC]
    public void PlayAttackAnimation(string clip)
    {
        playerAttackAnimator.Play(clip);
    }

    [PunRPC]
    public void PlayModelAnimation(string clip)
    {
        playerModelAnimator.Play(clip);
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

        // Update HealthBar and text
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
        GameUI.instance.UpdateHealthText();


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


            yield return new WaitForSeconds(0.1f);

            mr.material.color = defaultColor;
            flashingDamage = false;

            Debug.Log("Flash Off");

        }
    }

    [PunRPC]
    void Heal(int ammountToHeal)
    {
        curHp = Mathf.Clamp(curHp + ammountToHeal, 0, maxHp);

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GainCast()
    {
        ++castingCrystals;
        GameUI.instance.UpdateCastingCrystalText();
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
        if (cam.Target == this.transform)
            RoundManager.instance.StopSpectating();

        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        

        if (photonView.IsMine)
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
        }

    [PunRPC]
    public void AddKill()
    {
        kills++;
    }


    [PunRPC]
    void GenerateRandomAction(int playerId, string randomCardName, int randomDirection)
    {
        SpellCard randomCard = SpellRangeGenerator.instance.CardLibrary[randomCardName];
        if (randomCard.rangeIsDirectional)
            playerController.photonView.RPC("OnPrepareDirectionalCast", RpcTarget.All, playerId, randomCardName, randomDirection);
        else if (!randomCard.rangeIsDirectional)
            playerController.photonView.RPC("OnPrepareCast", RpcTarget.All, playerId, randomCardName);
    }

    public void PrepForNewRound()
    {
        turnCompleted = false;
        playerController.travelRange.Clear();
        playerController.myAction.card = null;
        playerController.myAction.effectRange = null;

        if (isStunned)
        {
            turnCompleted = true;

            if (photonView.IsMine)
            {
                GameUI.instance.ThrowNotification("You are stunned and unable to act this round");
                GameUI.instance.SetPlayerControls(false);
            }
            isStunned = false;
        }

        if (mirrorActive)
        {
            photonView.RPC("ToggleMirror", RpcTarget.All, false);
            mirrorActive = false;
        }

        if (isConfused)
        {
            if (isStunned)
            {
                if (photonView.IsMine)
                    GameUI.instance.ThrowNotification("You are stunned and unable to act this round");
            }
            else
            {
                if (photonView.IsMine)
                {
                    GameUI.instance.SetPlayerControls(false);
                    GameUI.instance.ThrowNotification("You are confused and will act unpredictably this round");
                    
                    if (PhotonNetwork.IsMasterClient)
                    {
                        string randomcardName = HandManager.instance.GetRandomCard();
                        int randomDirection = Random.Range(1, 5);
                        photonView.RPC("GenerateRandomAction", RpcTarget.All, id, randomcardName, randomDirection);
                        photonView.RPC("OnConfirmCast", RpcTarget.All, id);
                        Debug.Log(photonPlayer.NickName + " is casting a random spell: " + playerController.myAction.card.spellName); 
                    }
                }


            }

            isConfused = false;
        }

    }

}
