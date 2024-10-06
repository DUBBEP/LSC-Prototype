using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehavior : MonoBehaviour
{
    Vector2Int cords;
    private void Start()
    {
        cords = GridManager.instance.GetCoordinatesFromPosition(transform.position);
        GridManager.instance.GetTile(cords).crystal = this;
        GridManager.instance.PlaceCrystal(cords);
    }

    public void Collect(PlayerBehavior player)
    {
        GridManager.instance.CollectCrystal(cords);
        gameObject.SetActive(false);

        if (!PhotonNetwork.IsMasterClient)
            return;
        
        player.photonView.RPC("GainCast", player.photonPlayer);

    }
}
