using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    public string playerPrefabLocation;
    public PlayerBehavior[] players;
    public int alivePlayers;
    public Transform[] spawnPoints;
    public bool playersSpawned = false;


    private int playersInGame;




    // instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
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
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
