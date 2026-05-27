using UnityEngine;

/// <summary>
/// Permite al jugador agarrarse a lianas y balancearse como pendulo en el plano XY
/// (vista lateral 2.5D). Al tocar una liana en el aire se engancha automaticamente,
/// oscila bajo gravedad y, al pulsar ESPACIO, se suelta con el impulso del balanceo
/// para alcanzar la siguiente liana o la plataforma. El balanceo es analitico (no usa
/// joints) para no pelearse con el movimiento por Rigidbody de Player_Movement.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerSwing : MonoBehaviour
{
    [Tooltip("Movimiento normal que se desactiva mientras se balancea.")]
    public Player_Movement movimiento;

    [Header("Pendulo")]
    [Tooltip("Gravedad del pendulo (mas alto = balanceo mas rapido).")]
    public float gravedad = 20f;
    [Tooltip("Amortiguacion del balanceo (0 = no pierde energia).")]
    public float amortiguacion = 0.1f;

    [Header("Soltado")]
    [Tooltip("Impulso extra hacia arriba al soltar.")]
    public float impulsoVertical = 1.5f;
    [Tooltip("Multiplicador del impulso horizontal al soltar.")]
    public float impulsoHorizontal = 0.5f;
    [Tooltip("Velocidad horizontal minima de salida al soltar.")]
    public float salidaMinima = 5f;
    [Tooltip("Tiempo que se ignora la liana recien soltada para no reengancharla.")]
    public float tiempoReenganche = 0.5f;

    private Rigidbody rb;
    private bool balanceando;
    private Liana liana;
    private float angulo;      // radianes desde la vertical hacia abajo (+ hacia +X)
    private float velAngular;
    private float radio;
    private Vector3 pivote;
    private Liana ultimaLiana;
    private float tiempoIgnorar;

    public bool Balanceando { get { return balanceando; } }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (movimiento == null) movimiento = GetComponent<Player_Movement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (balanceando) return;
        var l = other.GetComponentInParent<Liana>();
        if (l == null) return;
        if (l == ultimaLiana && Time.time < tiempoIgnorar) return;
        Agarrar(l);
    }

    private void Agarrar(Liana l)
    {
        liana = l;
        liana.TomarControl();
        pivote = l.Pivote;
        radio = Mathf.Max(l.largo, 0.5f);

        Vector3 rel = transform.position - pivote;
        angulo = Mathf.Atan2(rel.x, -rel.y);

        Vector3 v = rb.linearVelocity;
        Vector3 tangente = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f);
        velAngular = Vector3.Dot(v, tangente) / radio;

        balanceando = true;
        rb.isKinematic = true;
        if (movimiento != null) movimiento.enabled = false;
        ColocarEnArco();
        liana.SetAngulo(angulo);
    }

    void Update()
    {
        if (balanceando && Input.GetKeyDown(KeyCode.Space))
            Soltar();
    }

    void FixedUpdate()
    {
        if (!balanceando) return;
        float dt = Time.fixedDeltaTime;
        float alpha = -(gravedad / radio) * Mathf.Sin(angulo) - amortiguacion * velAngular;
        velAngular += alpha * dt;
        angulo += velAngular * dt;
        ColocarEnArco();
        if (liana != null) liana.SetAngulo(angulo);
    }

    private void ColocarEnArco()
    {
        Vector3 pos = pivote + new Vector3(Mathf.Sin(angulo) * radio, -Mathf.Cos(angulo) * radio, 0f);
        pos.z = 0f;
        rb.MovePosition(pos);
    }

    private void Soltar()
    {
        ultimaLiana = liana;
        tiempoIgnorar = Time.time + tiempoReenganche;
        balanceando = false;
        if (liana != null) liana.SoltarControl();
        liana = null;

        rb.isKinematic = false;
        if (movimiento != null) movimiento.enabled = true;

        Vector3 tangente = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f);
        Vector3 vel = tangente * (velAngular * radio);

        // El nivel siempre progresa hacia +X: el soltado impulsa hacia adelante
        // con al menos salidaMinima, conservando el extra del balanceo si lo hubiera.
        float vx = Mathf.Max(Mathf.Abs(vel.x) * impulsoHorizontal, salidaMinima);
        float vy = Mathf.Max(vel.y, 0f) + impulsoVertical;
        rb.linearVelocity = new Vector3(vx, vy, 0f);
    }

    /// <summary>Aborta el balanceo sin impulso (p.ej. al reaparecer tras caer).</summary>
    public void Cancelar()
    {
        if (liana != null) liana.SoltarControl();
        liana = null;
        balanceando = false;
        if (rb != null) rb.isKinematic = false;
        if (movimiento != null) movimiento.enabled = true;
    }

}
