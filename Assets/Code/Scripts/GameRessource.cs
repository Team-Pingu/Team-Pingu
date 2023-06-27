using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public enum GameResourceType
{
    Tower,
    Minion,
    AutoMinion,
    Barricade,
    UI,
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

    public T LoadRessource<T>()
    {
        //#if UNITY_EDITOR
        //return AssetDatabase.LoadAssetAtPath<T>(AddressablesResourceKey);
        //#endif
        AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>(AddressablesResourceKey);
        opHandle.WaitForCompletion();

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            T prefab = opHandle.Result;
            return prefab;
        }
        else
        {
            Addressables.Release(opHandle);
            Debug.LogWarning($"Resource {AddressablesResourceKey} could not be loaded");
            return default(T);
        }
    }
}
