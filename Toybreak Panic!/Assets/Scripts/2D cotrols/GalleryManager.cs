using System.Collections;
using UnityEngine;

/// <summary>
/// Galeria de tiro del selector: cuando el jugador (ya con el blaster) entra en la
/// zona, activa los 3 goblins que se asoman. Al eliminar los 3 con el laser revela
/// el portal del Level 2 con un efecto de aparicion.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GalleryManager : MonoBehaviour
{
    public GoblinTarget[] goblins;
        [Tooltip("Muro que bloquea el avance; baja al limpiar la galeria.")]
    public Transform muro;
    [Tooltip("Cuanto baja el muro (unidades).")]
    public float muroBajada = 5f;
    [Tooltip("Duracion de la bajada del muro.")]
    public float muroDuracion = 1.2f;
    public string tagJugador = "Player";
    public bool requiereLaser = true;

    private int vivos;
    private bool iniciada;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Start()
    {
        foreach (var g in goblins)
        {
            if (g == null) continue;
            g.Init(this);
            g.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;
        Iniciar();
    }

    public void Iniciar()
    {
        if (iniciada) return;
        if (requiereLaser && !GameProgress.Instance.TieneLaser) return;

        iniciada = true;
        vivos = 0;
        foreach (var g in goblins)
        {
            if (g == null) continue;
            vivos++;
            g.Activar();
        }
    }

    public void GoblinEliminado()
    {
        vivos--;
        if (vivos <= 0) StartCoroutine(BajarMuro());
    }

    private IEnumerator BajarMuro()
    {
        if (muro == null) yield break;

        Vector3 ini = muro.position;
        Vector3 fin = ini + Vector3.down * muroBajada;
        float t = 0f;
        while (t < muroDuracion)
        {
            t += Time.deltaTime;
            muro.position = Vector3.Lerp(ini, fin, t / muroDuracion);
            yield return null;
        }
        muro.position = fin;

        var c = muro.GetComponent<Collider>();
        if (c != null) c.enabled = false;
    }
}
