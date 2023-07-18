using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Code.Scripts.Player.Controller;

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
    public void SpawnAttackerUnitServerRpc(Vector3 position) {
        GameObject gameObject = _attackerController.PlacePlaceholderUnit(position);
        gameObject.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnDefenderUnitServerRpc(Vector3 position) {
        GameObject gameObject = _defenderController.PlacePlaceholderUnit(position);
        gameObject.GetComponent<NetworkObject>().Spawn();
    }
}
