using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientStarter : NetworkManager
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting client");
        StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
