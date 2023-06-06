using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LogInstance : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(IsServer) {
            Debug.Log("Server started");
        }

        if(!IsServer) {
            Debug.Log("Not a server instance");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
