using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCollisionHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        Debug.Log("Enter");
        if (other.gameObject.tag == "Minion") {
            Debug.Log("Enter");
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("Exit");
        if (other.gameObject.tag == "Minion") {
            Debug.Log("Exit");
        }
    }
}
