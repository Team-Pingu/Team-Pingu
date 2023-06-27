using UnityEngine;
using Unity.Netcode;
using Code.Scripts.Player.Controller;

class ObjectSpawnManager : NetworkBehaviour {
    private PlayerController _attacker;
    private PlayerController _defender;
    
    private void Start() {
        if(IsClient) return;

        this.gameObject.AddComponent<AttackerPlayerController>();
        _attacker = this.gameObject.GetComponent<AttackerPlayerController>();
        _attacker.role = AttackerPlayerController.Role.Attacker;
        
        this.gameObject.AddComponent<DefenderPlayerController>();
        _defender = this.gameObject.AddComponent<DefenderPlayerController>();
        _defender.role = AttackerPlayerController.Role.Defender;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnObjectServerRpc(int role, Vector3 position) {
        Debug.Log("Server RPC Called");
        Debug.Log(role);
        if(role == 1) {
            GameObject gameObject = _attacker.PlaceTroops(position);
            gameObject.GetComponent<NetworkObject>().Spawn();
            return;
        }

        if(role == 2) {
            GameObject gameObject = _defender.PlaceTroops(position);
            gameObject.GetComponent<NetworkObject>().Spawn();
            return;
        }
    }
}