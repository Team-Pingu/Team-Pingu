using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum GameResourceType
{
    Tower,
    Minion,
    AutoMinion,
    Barricade,
    Undefined
}

public class GameResource
{
    public string AddressablesResourceKey { get; set; }
    public string ResourceID { get; set; }
    public GameResourceType ResourceType { get; set; }

    public GameResource(string resourceKey, string resourceID, GameResourceType resourceType)
    {
        AddressablesResourceKey = resourceKey;
        ResourceID = resourceID;
        ResourceType = resourceType;
    }

    public AsyncOperationHandle<GameObject> LoadRessource()
    {
        return Addressables.LoadAssetAsync<GameObject>(AddressablesResourceKey);
    }
}
