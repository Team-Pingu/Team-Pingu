using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Code.Scripts.GameManager.Role;

public class PlayerRoleManager : NetworkBehahviour
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
