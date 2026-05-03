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
    public float distanciaChequeoSuelo = 1.0f;
    public LayerMask capaSuelo;

    [Header("Jetpack")]
    public float velocidadJetpack = 5.0f;
    public JetpackParticulas jetpackParticulas;

    private bool estaEnSuelo;
    private bool saltoPendiente;
    private bool saltoRealizado;
    private bool jetpackActivo;

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

        // Chequeo de suelo: offset 0.1f arriba para evitar imprecision cuando el pivot esta al ras del suelo
        // Si capaSuelo no esta configurado en el Inspector, usa todas las capas
        int layerMask = (capaSuelo.value != 0) ? (int)capaSuelo : Physics.DefaultRaycastLayers;
        Vector3 rayOrigen = transform.position + Vector3.up * 0.1f;
        float rayLongitud = distanciaChequeoSuelo + 0.1f;
        estaEnSuelo = Physics.Raycast(rayOrigen, Vector3.down, rayLongitud, layerMask);
        Debug.DrawRay(rayOrigen, Vector3.down * rayLongitud, estaEnSuelo ? Color.green : Color.red);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            saltoPendiente = true;
            saltoRealizado = true;
        }

        // Jetpack
        jetpackActivo = Input.GetKey(KeyCode.LeftShift);

        // Enviar parametros al Animator
        // IsGrounded = false mientras saltoRealizado=true o jetpack activo
        move.SetBool("IsGrounded", estaEnSuelo && !saltoRealizado && !jetpackActivo);
        move.SetFloat("VerticalVelocity", rb_Jugador.linearVelocity.y);
        move.SetBool("JumpPressed", saltoRealizado);
        move.SetBool("Jetpack", jetpackActivo);

        jetpackParticulas?.SetActivo(jetpackActivo);
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

        // Jetpack: sobreescribe velocidad vertical mientras Shift este pulsado
        if (jetpackActivo)
        {
            Vector3 vel = rb_Jugador.linearVelocity;
            vel.y = velocidadJetpack;
            rb_Jugador.linearVelocity = vel;
        }

        // Si esta cayendo y no salto → caida libre
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
