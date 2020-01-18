using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class PlayerNetworkHandler : PlayerBehavior
{
    protected override void NetworkStart() {
        base.NetworkStart();

        GetComponent<ClientServerTogglables>().Init(networkObject.IsOwner);
    }

    void Update() {
        if (networkObject != null && !networkObject.IsOwner) {
            // put here for convenience, rather than an additional network movement class
            transform.position = networkObject.position;
        }
    }
}
