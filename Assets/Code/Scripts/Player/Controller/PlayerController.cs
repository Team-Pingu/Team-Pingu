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
        private Bank _bank;

        private void Start()
        {
            _bank = GetComponent<Bank>();
        }

        public Bank GetBank()
        {
            return _bank;
        }
        
        // Methode zum Platzieren der Truppen
        public abstract GameObject PlaceTroops(Vector3 position);
    }
}

