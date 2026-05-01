using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 5f;
    public float runMultiplier = 2f;
    public float gravity = -9.81f;
    public float jumHeight = 2f;
    public float rotationSpeed = 100f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isRunning = false;
    private bool isGrounded;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float rotateInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();

        // Suscripciones a eventos
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Run.performed += ctx => isRunning = true;
        inputActions.Player.Run.canceled += ctx => isRunning = false;

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Rotate.performed += ctx => rotateInput = ctx.ReadValue<float>();
        inputActions.Player.Rotate.canceled += ctx => rotateInput = 0f;
    }

    // CORRECCIÓN: OnEnable y OnDisable deben ir con mayúscula inicial
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumHeight * -2f * gravity);
        }
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movimiento horizontal
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move);
        float currentSpeed = isRunning ? speed * runMultiplier : speed;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Rotación
        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        // Gravedad y salto
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}