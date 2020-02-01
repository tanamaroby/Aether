﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
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
    private float m_FallingGravityMultiplier = 2.0f;

    [SerializeField]
    private float m_JumpHeight = 1.3f;

    private CharacterController m_CharacterController;
    private TextChanger m_textChanger;
    private PowerUpsManager m_PowerUps; 
    private VelocityModifier m_VelocityModifier; 

    private Vector3 m_Velocity;
    private Vector2 m_LastKnownInput;

    private float m_LandingRecoveryTime = 0.05f;
    private float m_LandingSpeedModifier = 0.0f;
    private float m_LandingTime = 0;
    private bool m_IsMidAir;
    private bool m_JumpedInCurrentFrame;
    private bool m_canDoubleSpeed;
    private bool m_canDoubleJump;
    private bool m_hasFlag;

    void Start()
    {
        AetherInput.GetPlayerActions().Jump.performed += HandleJump;
        m_CharacterController = GetComponent<CharacterController>();
        m_textChanger = GetComponent<TextChanger>();
        m_PowerUps = GetComponent<PowerUpsManager>();
        m_VelocityModifier = GetComponent<VelocityModifier>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetIsGrounded())
        {
            // Gravity should not accumulate when player is grounded. We set velocity to -2 instead of 0
            // because the collision may register before we actually fully touch the ground. This value is purely
            // empirical.
            if (m_Velocity.y < 0)
            {
                m_Velocity.y = -2.0f;
            }

            if (m_IsMidAir)
            {
                m_IsMidAir = false;
                // Landing frame
                m_LandingTime = Time.time;
            }
        }

        HandleGravity();
        HandleMovement();

        float t = Time.deltaTime;
        float t2 = t * t;

        float xVelocity = m_Velocity.x;
        float yVelocity = m_Velocity.y * t + 0.5f * GetGravityMagnitude() * t2; 
        float zVelocity = m_Velocity.z;

        xVelocity = m_VelocityModifier.ModifyXVelocity(xVelocity, m_PowerUps);
        yVelocity = m_VelocityModifier.ModifyYVelocity(yVelocity, m_PowerUps);
        zVelocity = m_VelocityModifier.ModifyZVelocity(zVelocity, m_PowerUps);

        m_CharacterController.Move(new Vector3(xVelocity, yVelocity, zVelocity));
    }

    private void LateUpdate()
    {
        m_JumpedInCurrentFrame = false;
    }

    private void HandleGravity()
    {
        m_Velocity.y += GetGravityMagnitude() * Time.deltaTime;
    }
    
    private void HandleMovement()
    {
        m_LastKnownInput = AetherInput.GetPlayerActions().Move.ReadValue<Vector2>();
        Vector3 move = Camera.main.transform.right * m_LastKnownInput.x + Camera.main.transform.forward * m_LastKnownInput.y;
        move *= Time.deltaTime * m_MoveSpeed;

        // Slow the player down after a fall
        if (IsRecoveringFromFall())
        {
            float mod = (Time.time - m_LandingTime) / m_LandingRecoveryTime;
            move *= Mathf.Lerp(m_LandingSpeedModifier, 1, mod);
        }

        m_Velocity = new Vector3(move.x, m_Velocity.y, move.z);
    }

    public void HandleJump(InputAction.CallbackContext ctx)
    {
        ButtonControl button = ctx.control as ButtonControl;
        if (!button.wasPressedThisFrame)
            return;

        if (!GetIsGrounded())
            return;

        if (IsRecoveringFromFall())
            return;

        m_JumpedInCurrentFrame = true;
        m_IsMidAir = true;
    }

    public Vector2 GetLastKnownInput()
    {
        return m_LastKnownInput;
    }

    public Vector3 GetVelocity()
    {
        return m_Velocity;
    }

    public float GetGravityMagnitude()
    {
        return m_Gravity * (m_Velocity.y >= 0 ? 1 : m_FallingGravityMultiplier);
    }

    public bool IsRecoveringFromFall()
    {
        return Time.time - m_LandingTime < m_LandingRecoveryTime;
    }

    public bool GetIsGrounded()
    {
        return Physics.CheckSphere(m_GroundCheck.position, 0.5f, m_LayerMask) && !GetJumpedInCurrentFrame();
    }

    public bool GetJumpedInCurrentFrame()
    {
        return m_JumpedInCurrentFrame;
    }

    // This should be an animation callback for more visually appealing jumps
    public void Jump()
    {
        m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2 * m_Gravity);
    }
}
