using System.Collections;
using UnityEngine;

/// <summary>
/// Maneja el arma (blaster) del personaje: la muestra solo cuando GameProgress.TieneLaser
/// esta desbloqueado, apunta hacia el cursor con clic derecho (IK del brazo derecho) y
/// dispara con clic izquierdo (fogonazo + tracer + raycast + retroceso).
/// El apuntado es procedural porque el rig no tiene clips dedicados de aim/shoot.
/// </summary>
[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform arma;          // Instancia del blaster (hija de la mano derecha)
    public Transform muzzle;        // Punta del canon (origen del disparo)
    public Light fogonazo;          // Luz que parpadea al disparar
    public LineRenderer tracer;     // Linea breve del disparo

    [Header("Apuntado")]
    [Tooltip("Plano de juego (Z) donde se proyecta el cursor.")]
    public float planoZ = 0f;
    [Tooltip("Largo del brazo al extenderse para apuntar.")]
    public float largoBrazo = 0.55f;
    [Range(0f, 1f)] public float pesoIK = 1f;
    [Tooltip("Suavizado del giro del personaje al apuntar.")]
    public float velocidadGiro = 18f;

    [Header("Disparo")]
    public float cadencia = 0.18f;
    public float alcance = 60f;
    public LayerMask capasImpacto = ~0;
    public float duracionTracer = 0.04f;
    [Tooltip("Radio del disparo: tolerancia para alcanzar objetivos en la vista 2.5D.")]
    public float radioDisparo = 0.5f;

    private Animator anim;
    private Transform hombroDer;
    private bool apuntando;
    private bool tieneLaser;
    private Vector3 objetivoMundo;
    private float proximoDisparo;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hombroDer = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        if (fogonazo != null) fogonazo.enabled = false;
        if (tracer != null) tracer.enabled = false;
    }

    void Update()
    {
        tieneLaser = GameProgress.Instance.TieneLaser;
        if (arma != null && arma.gameObject.activeSelf != tieneLaser)
            arma.gameObject.SetActive(tieneLaser);

        if (!tieneLaser) { apuntando = false; return; }

        var cam = Camera.main;
        if (cam == null) return;

        // Proyectar el cursor sobre el plano de juego (Z = planoZ)
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Mathf.Abs(ray.direction.z) > 0.0001f)
        {
            float t = (planoZ - ray.origin.z) / ray.direction.z;
            if (t > 0f) objetivoMundo = ray.origin + ray.direction * t;
        }

        apuntando = Input.GetMouseButton(1);

        if (apuntando && Input.GetMouseButtonDown(0) && Time.time >= proximoDisparo)
            Disparar();
    }

    void LateUpdate()
    {
        if (!tieneLaser || !apuntando || arma == null) return;

        // Mirar hacia el lado del cursor (izquierda/derecha) en la vista lateral
        float dx = objetivoMundo.x - transform.position.x;
        if (Mathf.Abs(dx) > 0.05f)
        {
            Quaternion objetivo = Quaternion.Euler(0f, dx >= 0f ? 90f : -90f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, velocidadGiro * Time.deltaTime);
        }

        // Orientar el arma: el canon (+Z) apunta al objetivo. Se fija la rotacion
        // mundial directamente (tras el IK) para evitar el offset del rig de la mano.
        Vector3 dir = objetivoMundo - arma.position;
        if (dir.sqrMagnitude > 0.0001f)
            arma.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    void OnAnimatorIK(int layer)
    {
        if (!tieneLaser || !apuntando || arma == null || hombroDer == null) return;

        // El brazo derecho se extiende desde el hombro hacia el objetivo
        Vector3 dir = objetivoMundo - hombroDer.position;
        if (dir.sqrMagnitude < 0.0001f) return;
        Vector3 manoPos = hombroDer.position + dir.normalized * largoBrazo;
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, pesoIK);
        anim.SetIKPosition(AvatarIKGoal.RightHand, manoPos);
    }

    private void Disparar()
    {
        proximoDisparo = Time.time + cadencia;

        Vector3 origen = muzzle != null ? muzzle.position : transform.position;
        Vector3 dir = objetivoMundo - origen;
        dir.z = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        dir.Normalize();

        Vector3 fin = origen + dir * alcance;
        RaycastHit hit;
        if (Physics.SphereCast(origen, radioDisparo, dir, out hit, alcance, capasImpacto, QueryTriggerInteraction.Collide))
        {
            fin = hit.point;
            var goblin = hit.collider.GetComponentInParent<GoblinTarget>();
            if (goblin != null) goblin.Impacto();
        }

        StartCoroutine(EfectoDisparo(origen, fin));
    }

    private IEnumerator EfectoDisparo(Vector3 origen, Vector3 fin)
    {
        if (fogonazo != null) fogonazo.enabled = true;
        if (tracer != null)
        {
            tracer.positionCount = 2;
            tracer.SetPosition(0, origen);
            tracer.SetPosition(1, fin);
            tracer.enabled = true;
        }
        yield return new WaitForSeconds(duracionTracer);
        if (fogonazo != null) fogonazo.enabled = false;
        if (tracer != null) tracer.enabled = false;
    }
}
