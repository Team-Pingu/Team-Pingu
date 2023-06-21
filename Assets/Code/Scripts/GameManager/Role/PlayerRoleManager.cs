using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.GameManager.Role;
using Unity.Netcode;

public class PlayerRoleManager : NetworkBehaviour
{


    private Role role;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(IsClient);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
