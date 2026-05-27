using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        // Event listener for the jump action
        onFoot.Jump.performed += ctx => motor.Jump();
    }

    void Update()
    {
        // Bloquea y oculta el cursor durante el juego; lo libera cuando el juego
        // esta en pausa (Time.timeScale == 0) para poder usar el menu de pausa.
        bool pausado = Time.timeScale == 0f;
        Cursor.lockState = pausado ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pausado;
    }

    void FixedUpdate()
    {
        // Process the movement of the player
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        // Process the rotation of the player
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
