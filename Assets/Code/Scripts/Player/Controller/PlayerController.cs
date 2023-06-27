using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Unity.Netcode;

namespace Code.Scripts.Player.Controller
{
    // Hauptklasse für den PlayerController
    public abstract class PlayerController : NetworkBehaviour
    {
        // Enum für die verschiedenen Rollen des Spielers
        public enum Role
        {
            Attacker,
            Defender,
            Spectator
        }

        public Role role;
        public GameObject playerUI;

        public GameObject playerUIInstance;
        
        private Bank _bank;

        private void Start()
        {
            LoadPrefabs();
            OnRoleChanged(role);
            _bank = gameObject.AddComponent<Bank>();
        }
        
        private void LoadPrefabs()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Assets/Level/UI/PlayerUI.prefab");
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                playerUI = handle.Result;
            } else {
                Debug.LogError("Das Laden des Prefabs ist fehlgeschlagen.");
                return;
            }
        }

        private void OnRoleChanged(Role newRole)
        {
            // // Aktualisieren des UIs, wenn sich die Rolle ändert
            // switch (newRole)
            // {
            //     case Role.Attacker:
            //         ActivateAttackerUI();
            //         break;
            //     case Role.Defender:
            //         ActivateDefenderUI();
            //         break;
            // }

            ShowUI();
        }

        private void ShowUI()
        {
            playerUIInstance = Instantiate(playerUI);
            Button button = playerUIInstance.transform.Find("Canvas/SwitchButton").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Destroy(playerUIInstance);
                Destroy(this);
            });
        }

        public Bank GetBank()
        {
            return _bank;
        }
        
        // Methode zum Platzieren der Truppen
        public abstract GameObject PlaceTroops(Vector3 position);
    }
}

