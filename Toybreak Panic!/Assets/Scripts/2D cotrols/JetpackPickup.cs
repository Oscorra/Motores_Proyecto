using UnityEngine;

/// <summary>
/// Jetpack suspendido en el aire (con bote y haz de luz). Al pasar el jugador por
/// encima, el modelo se atacha a su espalda y se desbloquea la habilidad del jetpack
/// en GameProgress (que Player_Movement ya consulta para volar con LeftShift).
/// </summary>
[RequireComponent(typeof(Collider))]
public class JetpackPickup : MonoBehaviour
{
    [Tooltip("Modelo del jetpack que flota y luego se atacha a la espalda.")]
    public Transform modeloJet;
    [Tooltip("Efecto (haz de luz) a ocultar al recoger.")]
    public GameObject efecto;
    public string tagJugador = "Player";

    [Header("Flotacion")]
    public float amplitud = 0.25f;
    public float velocidadBote = 2f;
    public float velocidadGiro = 40f;

    [Header("Atache a la espalda (local respecto al jugador)")]
    public Vector3 offsetEspalda = new Vector3(0f, 1.95f, -0.28f);
    public Vector3 eulerEspalda = new Vector3(0f, 0f, 0f);
    [Tooltip("Escala local en la espalda. <=0 = no tocar la escala.")]
    public float escalaEnEspalda = -1f;

    private Vector3 basePos;
    private bool recogido;

    void Start()
    {
        if (modeloJet != null) basePos = modeloJet.position;
        GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        if (recogido || modeloJet == null) return;
        float y = basePos.y + Mathf.Sin(Time.time * velocidadBote) * amplitud;
        modeloJet.position = new Vector3(basePos.x, y, basePos.z);
        modeloJet.Rotate(0f, velocidadGiro * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (recogido) return;
        if (!other.CompareTag(tagJugador)) return;

        recogido = true;
        GameProgress.Instance.DesbloquearJetpack();

        Transform espalda = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;
        if (modeloJet != null)
        {
            modeloJet.SetParent(espalda, true);
            modeloJet.localPosition = offsetEspalda;
            modeloJet.localEulerAngles = eulerEspalda;
            if (escalaEnEspalda > 0f) modeloJet.localScale = Vector3.one * escalaEnEspalda;
        }

        if (efecto != null) efecto.SetActive(false);
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
