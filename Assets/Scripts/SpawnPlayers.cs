using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public float spawnRadius = 100f;

    void Start()
    {
        Vector3 randomSpawnPoint = Random.insideUnitSphere * spawnRadius;
        randomSpawnPoint.y = 0f;
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPoint, Quaternion.identity);
    }
}
