using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ServerStarter : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Starting Server");
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
