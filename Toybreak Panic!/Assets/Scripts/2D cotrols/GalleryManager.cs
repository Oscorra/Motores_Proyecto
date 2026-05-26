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
    public GameObject portalLevel2;
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
        if (portalLevel2 != null) portalLevel2.SetActive(false);
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
        if (vivos <= 0) StartCoroutine(AbrirPortal());
    }

    private IEnumerator AbrirPortal()
    {
        if (portalLevel2 == null) yield break;

        portalLevel2.SetActive(true);
        Vector3 plena = portalLevel2.transform.localScale;
        portalLevel2.transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            portalLevel2.transform.localScale = Vector3.Lerp(Vector3.zero, plena, t / 0.5f);
            yield return null;
        }
        portalLevel2.transform.localScale = plena;
    }
}
