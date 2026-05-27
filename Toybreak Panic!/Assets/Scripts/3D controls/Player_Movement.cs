using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private MeshRenderer m_Renderer_Jugador;
    private Rigidbody rb_Jugador;

    public float multiplicadorDesplazamiento = 8.0f;
    public float multiplicadorSprint = 1.8f;    // Cuanto más rápido corre físicamente
    public float multiplicadorAnimacion = 1.4f; // Cuanto más rápido se mueve la animación
    public Transform cameraTransform;

    [Header("Movimiento lateral 2D")]
    public bool movimientoLateral2D = false;
    public bool bloquearProfundidadEn2D = true;
    public float profundidad2D = 0f;

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

    [Header("Hielo")]
    public PhysicsMaterial materialHielo;
    private float friccionActual = 1f;
    private float friccionObjetivo = 1f;

    [Header("Hielo")]
    private Vector3 rayOrigen;
    private float rayLongitud;
    private int layerMask;

    private bool estaEnSuelo;
    private bool saltoPendiente;
    private bool saltoRealizado;
    private bool jetpackActivo;
    private bool sprintActivo;


    void Start()
    {
        m_Renderer_Jugador = GetComponent<MeshRenderer>();
        rb_Jugador = GetComponent<Rigidbody>();
        move = GetComponent<Animator>();
        profundidad2D = transform.position.z;

    }

    void Update()
    {
        // Movimiento
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        if (movimientoLateral2D)
        {
            direccionMovimiento = Vector3.right * inputX;
        }
        else
        {
            Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;
            camRight.y = 0;
            camRight.Normalize();

            direccionMovimiento = (camForward * inputZ + camRight * inputX);
        }

        // Chequeo de suelo: offset 0.1f arriba para evitar imprecision cuando el pivot esta al ras del suelo
        // Si capaSuelo no esta configurado en el Inspector, usa todas las capas
        layerMask = (capaSuelo.value != 0) ? (int)capaSuelo : Physics.DefaultRaycastLayers;
        rayOrigen = transform.position + Vector3.up * 0.1f;
        rayLongitud = distanciaChequeoSuelo + 0.1f;
        estaEnSuelo = Physics.Raycast(rayOrigen, Vector3.down, rayLongitud, layerMask);
        Debug.DrawRay(rayOrigen, Vector3.down * rayLongitud, estaEnSuelo ? Color.green : Color.red);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            saltoPendiente = true;
            saltoRealizado = true;
        }

        // Jetpack: solo responde si el jugador lo tiene desbloqueado
        jetpackActivo = Input.GetKey(KeyCode.LeftShift) && GameProgress.Instance.TieneJetpack;

        sprintActivo = Input.GetKey(KeyCode.LeftControl) && estaEnSuelo && direccionMovimiento.sqrMagnitude > 0.1f;

        // Enviar parametros al Animator
        // IsGrounded = false mientras saltoRealizado=true o jetpack activo
        move.SetBool("IsGrounded", estaEnSuelo && !saltoRealizado && !jetpackActivo);
        move.SetFloat("VerticalVelocity", rb_Jugador.linearVelocity.y);
        move.SetBool("JumpPressed", saltoRealizado);
        move.SetBool("Jetpack", jetpackActivo);
        move.SetBool("IsRunning", sprintActivo);

        jetpackParticulas?.SetActivo(jetpackActivo);
    }

    void FixedUpdate()
    {
        //float mFisico = sprintActivo ? multiplicadorSprint : 1.0f;
        float mFisico = (Input.GetKey(KeyCode.LeftControl) && estaEnSuelo) ? multiplicadorSprint : 1.0f;
        // Detectar si está en hielo
        RaycastHit hit;
        if (Physics.Raycast(rayOrigen, Vector3.down, out hit, rayLongitud, layerMask))
        {
            if (hit.collider.sharedMaterial == materialHielo)
                friccionObjetivo = 0.05f;
            else
                friccionObjetivo = 1f;
        }

        friccionActual = Mathf.Lerp(friccionActual, friccionObjetivo, Time.fixedDeltaTime * 3f);

        // Movimiento
        if (direccionMovimiento.sqrMagnitude > 0.1f)
        {
            /*
            direccionMovimiento.Normalize();

            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 0.2f);



            Vector3 direccion = rb_Jugador.position + direccionMovimiento * multiplicadorDesplazamiento * Time.fixedDeltaTime;

            rb_Jugador.MovePosition(direccion);
            */
            // IMPORTANTE: Primero normalizamos para tener dirección pura

            if (direccionMovimiento.sqrMagnitude > 0.1f)
            {
                direccionMovimiento.Normalize();
                Quaternion rotacionObjetivo = movimientoLateral2D
                    ? Quaternion.Euler(0f, direccionMovimiento.x >= 0f ? 90f : -90f, 0f)
                    : Quaternion.LookRotation(direccionMovimiento, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 0.2f);

                // Mover al personaje
                Vector3 direccion = rb_Jugador.position + direccionMovimiento * (multiplicadorDesplazamiento * mFisico) * Time.fixedDeltaTime;
                if (movimientoLateral2D && bloquearProfundidadEn2D)
                    direccion.z = profundidad2D;
                rb_Jugador.MovePosition(direccion);
            }
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

        //Movimiento local para el blend tree de caminar/correr
        Vector3 movimientoLocal = transform.InverseTransformDirection(direccionMovimiento);
        move.SetFloat("VelZ", movimientoLocal.z);
        move.SetFloat("VelX", movimientoLocal.x);
        

        /* Enviamos el multiplicador al Animator (Asegúrate de crear el parámetro "AnimSpeedMultiplier")
        if (move != null)
        {
            Vector3 movimientoLocal = transform.InverseTransformDirection(direccionMovimiento);

            // Al multiplicar VelZ y VelX por mFisico, el valor sube de 1 a 1.5
            // Esto hará que el Blend Tree se desplace hacia la animación de correr
            move.SetFloat("VelZ", movimientoLocal.z * mFisico);
            move.SetFloat("VelX", movimientoLocal.x * mFisico);
        }*/
    }
}
