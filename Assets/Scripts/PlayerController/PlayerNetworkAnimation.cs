using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private AnimationCurve m_AnimationSpeedCurve;

    private PlayerNetworkHandler m_PlayerNetworkHandler;

    void Start()
    {
        m_PlayerNetworkHandler = GetComponent<PlayerNetworkHandler>();
    }

    void Update()
    {
        if (m_PlayerNetworkHandler.networkObject == null)
            return;

        float worldVelocity = m_PlayerNetworkHandler.networkObject.velocity;
        transform.rotation = m_PlayerNetworkHandler.networkObject.rotation;
        m_Animator.SetFloat("Velocity", worldVelocity);
        m_Animator.SetFloat("VerticalVelocity", m_PlayerNetworkHandler.networkObject.vertVelocity);
        m_Animator.SetBool("Grounded", m_PlayerNetworkHandler.networkObject.grounded);

        if (worldVelocity > 0.01f)
            m_Animator.speed = m_AnimationSpeedCurve.Evaluate(worldVelocity); // Make running animation speed match actual movement speed
        else
            m_Animator.speed = 1;
    }
}
