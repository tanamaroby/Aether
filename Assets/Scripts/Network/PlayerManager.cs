using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

public class PlayerManager : MonoBehaviour
{
    private static PlayerBehavior localPlayer;

    void Start()
    {
        LoadPlayer();
    }

    public static void LoadPlayer()
    {
        Vector3 spawnPosition = Vector3.one;
        localPlayer = NetworkManager.Instance.InstantiatePlayer(position: spawnPosition);
    }

    public static PlayerBehavior GetLocalPlayerInstance()
    {
        return localPlayer;
    }

    private static void DestroyPlayer(PlayerBehavior player)
    {
        Destroy(player);
    }
}
