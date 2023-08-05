using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Code.Scripts.Player.Controller;
using System;
using Code.Scripts;

public class ObjectSpawner : NetworkBehaviour {
    private PlayerController _attackerController;
    private PlayerController _defenderController;
    // Start is called before the first frame update
    void Start()
    {
        _attackerController = this.gameObject.AddComponent<AttackerPlayerController>();
        _defenderController = this.gameObject.AddComponent<DefenderPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnAttackerUnitServerRpc(String prefabName, Vector3 position, float followPathDelay) {
        GameObject gameObject = _attackerController.PlaceUnit(prefabName, position);
        gameObject.GetComponent<NetworkObject>().Spawn();
        gameObject.GetComponent<MinionMover>().SetDelay(followPathDelay);
        gameObject.GetComponent<MinionMover>().StartFollowing();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnDefenderUnitServerRpc(String prefabName, Vector3 position) {
        GameObject gameObject = _defenderController.PlaceUnit(prefabName, position);
        gameObject.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Defender Object spawned");
    }

    [ServerRpc(RequireOwnership = false)]
    public void TestServerRpc(String test) {
        Debug.Log("Test server rpc");
    }
}
