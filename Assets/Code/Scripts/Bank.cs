using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts
{
    public class Bank : MonoBehaviour
    {
        [SerializeField] public int startingBalance = 1500;

        public int CurrentBalance { get { return _currentBalance; } }

        [SerializeField] private int _currentBalance;

        // custom event handler for balance change
        public event Action<int> OnBalanceChanged;

        public void Awake()
        {
            _currentBalance = startingBalance;
        }

        public void Deposit(int amount)
        {
            _currentBalance += Mathf.Abs(amount);
            OnBalanceChanged(_currentBalance);
        }

        public void Withdraw(int amount)
        {
            _currentBalance -= Mathf.Abs(amount);
            OnBalanceChanged(_currentBalance);

            if (_currentBalance < 0)
            {
                // TODO: handle action when balance is negative!
                //ReloadScene();
            }
        }

        private void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
