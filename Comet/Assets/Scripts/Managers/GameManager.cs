using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    public string playerPrefabLocation;
    public PlayerBehavior[] players;
    public int alivePlayers;
    public Transform[] spawnPoints;
    public bool playersSpawned = false;

    public Vector3 camDefaultPos;

    [SerializeField]
    private float postGameTime;


    private int playersInGame;




    // instance
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            gameObject.SetActive(false);
    }

    private void Start()
    {
        players = new PlayerBehavior[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;
        playersSpawned = false;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);

        // initialize the player for all other players
        playerObj.GetComponent<PlayerBehavior>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        Invoke("SetSpawnedTrue", 0.5f);
    }

    public PlayerBehavior GetPlayer(int playerId)
    {
        foreach (PlayerBehavior player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }
        return null;
    }

    public PlayerBehavior GetPlayer(GameObject playerObject)
    {
        foreach (PlayerBehavior player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }
        return null;
    }


    public void CheckWinCondition()
    {
        if (alivePlayers == 1)
        {
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        }
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        // set the UI win text
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);

        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        Destroy(NetworkManager.instance.gameObject);
        NetworkManager.instance.ChangeScene("Menu");
    }

    void SetSpawnedTrue()
    {
        playersSpawned = true;
    }
}
