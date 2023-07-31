using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using Unity.Netcode;

namespace Code.Scripts.Player.Controller
{
    // Enum für die verschiedenen Rollen des Spielers
    public enum PlayerRole
    {
        Attacker,
        Defender
    }

    public class Player : MonoBehaviour
    {
        [SerializeField]
        public PlayerRole Role;
        
        public PlayerController PlayerController;

        private List<String> activeEntities;

        private void Start()
        {  
            #if !UNITY_EDITOR
            Role = PlayerRole.Attacker;
            if(NetworkManager.Singleton.LocalClientId == 2) Role = PlayerRole.Attacker;
            #endif

            activeEntities = new List<string>();

            if (Role == PlayerRole.Defender) PlayerController = gameObject.AddComponent<DefenderPlayerController>();
            else PlayerController = gameObject.AddComponent<AttackerPlayerController>();

            if(Role == PlayerRole.Attacker) {
                this.AddToActiveEntity("AetherShieldBearer");
                this.AddToActiveEntity("GearheadSapper");
            }

            if(Role == PlayerRole.Defender) {
                this.AddToActiveEntity("BoltThrower");
            }
        }

        private void Update()
        {
        }

        public void AddToActiveEntity(String prefabName) {
            this.activeEntities.Add(prefabName);
        }

        public void RemoveFromActiveEntity(String prefabName) {
            this.activeEntities.Remove(prefabName);
        }

        public List<String> GetActiveEntity() {
            return this.activeEntities;
        }

        public void ClearActiveEntity() {
            this.activeEntities.Clear();
        }

        public void selectCard() {
            
        }
    }
}