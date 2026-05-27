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
    [Tooltip("En el selector 2D el blaster solo apunta a izquierda/derecha en el eje X global.")]
    public bool apuntarSoloEjeXGlobal = true;

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

        objetivoMundo = GetObjetivoApuntado();

        apuntando = Input.GetMouseButton(1);

        if (apuntando && Input.GetMouseButtonDown(0) && Time.time >= proximoDisparo)
            Disparar();
    }

    void LateUpdate()
    {
        if (!tieneLaser || !apuntando || arma == null) return;

        // El canon se inclina en el plano XY, pero su X siempre queda hacia el lado
        // al que ya mira el jugador. El cursor solo aporta altura/inclinacion.
        Vector3 dir = GetDireccionDisparo();
        dir.z = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            arma.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    void OnAnimatorIK(int layer)
    {
        if (!tieneLaser || !apuntando || arma == null || hombroDer == null) return;

        // El brazo derecho se extiende hacia el lado al que ya mira el jugador.
        Vector3 dir = GetDireccionDisparo();
        dir.z = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Vector3 manoPos = hombroDer.position + dir.normalized * largoBrazo;
        manoPos.z = planoZ;
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, pesoIK);
        anim.SetIKPosition(AvatarIKGoal.RightHand, manoPos);
    }

    private void Disparar()
    {
        proximoDisparo = Time.time + cadencia;

        Vector3 origen = muzzle != null ? muzzle.position : transform.position;
        origen.z = planoZ;
        Vector3 dir = GetDireccionDisparo();
        dir.Normalize();

        Vector3 fin = origen + dir * alcance;
        fin.z = planoZ;
        RaycastHit hit;
        if (Physics.SphereCast(origen, radioDisparo, dir, out hit, alcance, capasImpacto, QueryTriggerInteraction.Collide))
        {
            fin = hit.point;
            fin.z = planoZ;
            var goblin = hit.collider.GetComponentInParent<GoblinTarget>();
            if (goblin != null) goblin.Impacto();
        }

        StartCoroutine(EfectoDisparo(origen, fin));
    }

    private Vector3 GetObjetivoApuntado()
    {
        var cam = Camera.main;
        if (cam == null) return objetivoMundo;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Mathf.Abs(ray.direction.z) <= 0.0001f) return objetivoMundo;

        float t = (planoZ - ray.origin.z) / ray.direction.z;
        return t > 0f ? ray.origin + ray.direction * t : objetivoMundo;
    }

    private Vector3 GetDireccionDisparo()
    {
        if (!apuntarSoloEjeXGlobal)
        {
            Vector3 dir = objetivoMundo - (muzzle != null ? muzzle.position : transform.position);
            dir.z = 0f;
            return dir.sqrMagnitude > 0.0001f ? dir.normalized : transform.forward;
        }

        Vector3 origen = muzzle != null ? muzzle.position : transform.position;
        origen.z = planoZ;

        float lado = transform.forward.x >= 0f ? 1f : -1f;
        float distanciaHorizontal = Mathf.Abs(objetivoMundo.x - origen.x);
        distanciaHorizontal = Mathf.Max(distanciaHorizontal, 0.25f);

        Vector3 objetivoBloqueado = objetivoMundo;
        objetivoBloqueado.x = origen.x + distanciaHorizontal * lado;
        objetivoBloqueado.z = planoZ;

        Vector3 direccion = objetivoBloqueado - origen;
        direccion.z = 0f;
        return direccion.sqrMagnitude > 0.0001f ? direccion.normalized : (lado > 0f ? Vector3.right : Vector3.left);
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
