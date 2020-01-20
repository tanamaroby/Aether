using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private AnimationCurve m_AnimationSpeedCurve;

    private PlayerMovement m_PlayerMovement;

    private PlayerNetworkHandler m_PlayerNetworkHandler;

    void Start()
    {
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_PlayerNetworkHandler = GetComponent<PlayerNetworkHandler>();
    }

    void Update()
    {
        float worldVelocity = m_PlayerMovement.GetAbsInput();
        float vertVelocity = m_PlayerMovement.GetVerticalVelocity();
        m_Animator.SetFloat("Velocity", worldVelocity);
        m_Animator.SetFloat("VerticalVelocity", vertVelocity);

        Vector3 movementDir = m_PlayerMovement.GetMovementDirection();
        if (movementDir.magnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementDir), Time.deltaTime * 10);

        bool isGrounded = m_PlayerMovement.GetIsGrounded();

        m_Animator.SetBool("Grounded", isGrounded);

        if (m_PlayerMovement.GetJumpedInCurrentFrame())
            m_Animator.SetTrigger("Jumping");

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            if (worldVelocity > 0.01f)
                m_Animator.speed = m_AnimationSpeedCurve.Evaluate(worldVelocity); // Make running animation speed match actual movement speed
            else
                m_Animator.speed = 1;
        }

        if (m_PlayerNetworkHandler.networkObject != null)
        {
            m_PlayerNetworkHandler.networkObject.velocity = worldVelocity;
            m_PlayerNetworkHandler.networkObject.vertVelocity = vertVelocity;
            m_PlayerNetworkHandler.networkObject.rotation = transform.rotation;
            m_PlayerNetworkHandler.networkObject.grounded = isGrounded;
        }
    }
}
