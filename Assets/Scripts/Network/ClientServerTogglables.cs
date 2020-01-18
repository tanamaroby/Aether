using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class ClientServerTogglables : MonoBehaviour
{
    [Header("Local References")]
    [SerializeField]
    private GameObject m_LocalTogglable;
    [SerializeField]
    private Component[] m_LocalTogglableScripts;

    [Header("Network References")]
    [SerializeField]
    private GameObject m_NetworkTogglable;
    [SerializeField]
    private Component[] m_NetworkTogglableScripts;

    public void Init(bool isOwner)
    {
        if (isOwner)
            Destroy(m_NetworkTogglable);
        else
            Destroy(m_LocalTogglable);

        foreach (Component script in m_LocalTogglableScripts)
        {
            if (!isOwner)
                Destroy(script);
        }

        foreach (Component script in m_NetworkTogglableScripts)
        {
            if (isOwner)
                Destroy(script);
        }
    }
}
