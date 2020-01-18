﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10)]
    private float m_MoveSpeed = 6;

    [SerializeField]
    private Transform m_GroundCheck = null;

    [SerializeField]
    private LayerMask m_LayerMask = new LayerMask();

    [SerializeField]
    private float m_Gravity = -9.8f;

    [SerializeField]
    private float m_JumpHeight = 1.3f;

    private CharacterController m_CharacterController;

    private PlayerNetworkHandler m_PlayerNetworkHandler;

    private Vector3 m_Velocity;

    private float m_PlayerHeight;

    private float m_LandingRecoveryTime = 0.05f;
    private float m_LandingSpeedModifier = 0.0f;
    private float m_LandingTime = 0;
    private bool m_IsMidAir;
    private bool m_JumpedInCurrentFrame;

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerNetworkHandler = GetComponent<PlayerNetworkHandler>();
        m_PlayerHeight = m_CharacterController.height;
    }

    // Update is called once per frame
    void Update()
    {
        m_JumpedInCurrentFrame = false;
        bool isGrounded = GetIsGrounded();

        if (isGrounded)
        {
            if (m_Velocity.y < 0)
            {
                m_Velocity.y = -2f;
            }

            if (m_IsMidAir)
            {
                m_IsMidAir = false;
                // Landing frame
                m_LandingTime = Time.time;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Camera.main.transform.right * x + Camera.main.transform.forward * z;
        move *= Time.deltaTime * m_MoveSpeed;

        // Slow the player down after a fall
        bool isRecoveringFromFall = Time.time - m_LandingTime < m_LandingRecoveryTime;
        if (isRecoveringFromFall)
        {
            float mod = (Time.time - m_LandingTime) / m_LandingRecoveryTime;
            move *= Mathf.Lerp(m_LandingSpeedModifier, 1, mod);
        }

        m_CharacterController.Move(move);

        if (Input.GetButtonDown("Jump") && isGrounded && !isRecoveringFromFall)
        {
            m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2 * m_Gravity);
            m_JumpedInCurrentFrame = true;
            m_IsMidAir = true;
        }

        m_Velocity.y += m_Gravity * Time.deltaTime;
        m_CharacterController.Move(m_Velocity * Time.deltaTime);


        if (Input.GetKey(KeyCode.LeftControl))
            m_CharacterController.height = m_PlayerHeight / 2;
        else
            m_CharacterController.height = m_PlayerHeight;

        if (m_PlayerNetworkHandler.networkObject != null)
            m_PlayerNetworkHandler.networkObject.position = transform.position;
    }

    public float GetAbsInput()
    {
        return Mathf.Clamp01(Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")));
    }

    public Vector3 GetMovementDirection()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Camera.main.transform.right * x + Camera.main.transform.forward * z;
        move.y = 0;
        return move.normalized;
    }

    public bool GetIsGrounded()
    {
        return Physics.CheckSphere(m_GroundCheck.position, 0.2f, m_LayerMask);
    }

    public bool GetJumpedInCurrentFrame()
    {
        return m_JumpedInCurrentFrame;
    }
}
