using System.Collections;
using UnityEngine;

/// <summary>
/// Goblin "Chupin" de la galeria de tiro del selector. Se asoma (aparece) a una
/// altura aleatoria con efecto, permanece visible un rato y se oculta, repitiendo
/// el ciclo hasta que el laser del blaster lo alcanza: entonces muere con efecto
/// y avisa al GalleryManager.
/// </summary>
public class GoblinTarget : MonoBehaviour
{
    [Header("Asomarse")]
    public Vector2 rangoAltura = new Vector2(1.2f, 4.2f);
        [Tooltip("Tiempo visible: aleatorio entre min y max cada ciclo.")]
    public Vector2 rangoTiempoVisible = new Vector2(0.7f, 1.8f);
        [Tooltip("Tiempo oculto entre asomadas: aleatorio cada ciclo.")]
    public Vector2 rangoTiempoOculto = new Vector2(0.5f, 1.8f);
    [Tooltip("Retardo inicial aleatorio para desincronizar de los demas.")]
    public Vector2 rangoRetardoInicial = new Vector2(0f, 1.6f);
    public float velocidadEscala = 8f;

    [Header("Efecto")]
    public Color colorFlash = new Color(0.5f, 0.95f, 1f);

    private GalleryManager manager;
    private Collider col;
    private Light flash;
    private Vector3 escalaPlena;
    private bool vivo = true;
    private bool visible;

    public void Init(GalleryManager m) { manager = m; }

    void Awake()
    {
        escalaPlena = transform.localScale;
        transform.localScale = Vector3.zero;
        col = GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        var flashGO = new GameObject("Flash");
        flashGO.transform.SetParent(transform, false);
        flashGO.transform.localPosition = Vector3.up * 0.8f;
        flash = flashGO.AddComponent<Light>();
        flash.type = LightType.Point;
        flash.color = colorFlash;
        flash.range = 6f;
        flash.intensity = 0f;
    }

    public void Activar()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Ciclo());
    }

    private IEnumerator Ciclo()
    {
        // Retardo inicial aleatorio: cada goblin arranca su ciclo por su cuenta
        yield return new WaitForSeconds(Random.Range(rangoRetardoInicial.x, rangoRetardoInicial.y));
        while (vivo)
        {
            float y = Random.Range(rangoAltura.x, rangoAltura.y);
            transform.position = new Vector3(transform.position.x, y, 0f);

            StartCoroutine(Flash(0.15f, 4f));
            yield return EscalarA(escalaPlena);
            if (col != null) col.enabled = true;
            visible = true;

            float visibleDur = Random.Range(rangoTiempoVisible.x, rangoTiempoVisible.y);
            float t = 0f;
            while (t < visibleDur && vivo) { t += Time.deltaTime; yield return null; }
            if (!vivo) yield break;

            visible = false;
            if (col != null) col.enabled = false;
            yield return EscalarA(Vector3.zero);

            yield return new WaitForSeconds(Random.Range(rangoTiempoOculto.x, rangoTiempoOculto.y));
        }
    }

    private IEnumerator EscalarA(Vector3 destino)
    {
        while ((transform.localScale - destino).sqrMagnitude > 0.0001f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, destino, velocidadEscala * escalaPlena.x * Time.deltaTime);
            yield return null;
        }
        transform.localScale = destino;
    }

    private IEnumerator Flash(float dur, float intensidad)
    {
        if (flash == null) yield break;
        flash.intensity = intensidad;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            flash.intensity = Mathf.Lerp(intensidad, 0f, t / dur);
            yield return null;
        }
        flash.intensity = 0f;
    }

    /// <summary>Llamado por el blaster cuando el laser alcanza al goblin.</summary>
    public void Impacto()
    {
        if (!vivo || !visible) return;
        vivo = false;
        visible = false;
        if (col != null) col.enabled = false;
        StopAllCoroutines();
        StartCoroutine(Morir());
    }

    private IEnumerator Morir()
    {
        StartCoroutine(Flash(0.3f, 9f));
        float t = 0f;
        Vector3 ini = transform.localScale;
        Vector3 punch = escalaPlena * 1.4f;
        while (t < 0.1f) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(ini, punch, t / 0.1f); transform.Rotate(0f, 720f * Time.deltaTime, 0f); yield return null; }
        t = 0f;
        while (t < 0.25f) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(punch, Vector3.zero, t / 0.25f); transform.Rotate(0f, 1080f * Time.deltaTime, 0f); yield return null; }
        if (manager != null) manager.GoblinEliminado();
        Destroy(gameObject);
    }
}
