using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts
{
    public class Bank : MonoBehaviour
    {
        [SerializeField] public int startingBalance = 150;

        public int CurrentBalance { get { return _currentBalance; } }

        [SerializeField] private int _currentBalance;

        public void Awake()
        {
            _currentBalance = startingBalance;
        }

        public void Deposit(int amount)
        {
            _currentBalance += Mathf.Abs(amount);
        }

        public void Withdraw(int amount)
        {
            _currentBalance -= Mathf.Abs(amount);

            if (_currentBalance < 0)
            {
                // lose game or whatever
                // TODO: change if multiplayer is invented
                ReloadScene();
            }
        }

        private void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
