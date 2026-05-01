using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private MeshRenderer m_Renderer_Jugador;
    private Rigidbody rb_Jugador;

    public float multiplicadorDesplazamiento = 8.0f;
    public Transform cameraTransform;

    private Animator move;

    public float inputX { get; private set; }
    public float inputZ { get; private set; }
    private Vector3 direccionMovimiento;

    [Header("Salto")]
    public float fuerzaSalto = 6.0f;
    public float distanciaChequeoSuelo = 0.2f;
    public LayerMask capaSuelo;

    private bool estaEnSuelo;
    private bool saltoPendiente;
    private bool saltoRealizado;

    void Start()
    {
        m_Renderer_Jugador = GetComponent<MeshRenderer>();
        rb_Jugador = GetComponent<Rigidbody>();
        move = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        direccionMovimiento = (camForward * inputZ + camRight * inputX);

        // Chequeo de suelo
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaChequeoSuelo, capaSuelo);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            saltoPendiente = true;
            saltoRealizado = true; // Esto indica que el salto fue intencional
        }

        // Enviar par�metros al Animator
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, 0.2f, capaSuelo);
        move.SetBool("IsGrounded", estaEnSuelo);
        move.SetFloat("VerticalVelocity", rb_Jugador.linearVelocity.y);
        move.SetBool("JumpPressed", saltoRealizado);
    }

    void FixedUpdate()
    {
        // Movimiento
        if (direccionMovimiento.sqrMagnitude > 0.1f)
        {
            direccionMovimiento.Normalize();
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 0.2f);

            Vector3 direccion = rb_Jugador.position + direccionMovimiento * multiplicadorDesplazamiento * Time.fixedDeltaTime;
            rb_Jugador.MovePosition(direccion);
        }

        // Aplicar salto
        if (saltoPendiente)
        {
            rb_Jugador.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            saltoPendiente = false;
        }

        // Si est� cayendo y no salt� ? ca�da libre
        if (!estaEnSuelo && rb_Jugador.linearVelocity.y < 0 && !saltoRealizado)
        {
            move.SetBool("JumpPressed", false);
        }

        // Si aterriza ? resetear salto
        if (estaEnSuelo && saltoRealizado)
        {
            saltoRealizado = false;
            move.SetTrigger("Landing");
        }

        // Movimiento local para el blend tree de caminar/correr
        Vector3 movimientoLocal = transform.InverseTransformDirection(direccionMovimiento);
        move.SetFloat("VelZ", movimientoLocal.z);
        move.SetFloat("VelX", movimientoLocal.x);
    }
}
