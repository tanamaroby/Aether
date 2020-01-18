using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Instance.InstantiatePlayer(position: new Vector3(2, 1, -14));
    }
}
