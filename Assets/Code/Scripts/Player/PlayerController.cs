using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Code.Scripts.Player
{
    // Hauptklasse für den PlayerController
    public abstract class PlayerController : MonoBehaviour
    {
        // Enum für die verschiedenen Rollen des Spielers
        public enum Role
        {
            Attacker,
            Defender
        }

        public Role role;
        public GameObject playerUI;

        public GameObject playerUIInstance;

        private void Start()
        {
            LoadPrefabs();
            OnRoleChanged(role);
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
        
        // Methode zum Platzieren der Truppen
        public abstract void PlaceTroops(Vector3 position);
    }
}

