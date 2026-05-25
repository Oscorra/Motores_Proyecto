using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Muestra el mensaje pendiente de GameProgress al cargar la escena (p.ej. al volver
/// del Dungeon). Hace fade-in, espera y fade-out. Si no hay mensaje, se mantiene oculto.
/// </summary>
public class LevelMessageUI : MonoBehaviour
{
    public CanvasGroup grupo;
    public TMP_Text texto;
    public float duracion = 5f;
    public float fade = 0.5f;

    private void Start()
    {
        if (grupo != null) grupo.alpha = 0f;

        string m = GameProgress.Instance.ConsumirMensaje();
        if (string.IsNullOrEmpty(m)) return;

        if (texto != null) texto.text = m;
        StartCoroutine(Mostrar());
    }

    private IEnumerator Mostrar()
    {
        yield return Fade(0f, 1f);
        yield return new WaitForSeconds(duracion);
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float desde, float hasta)
    {
        if (grupo == null) yield break;

        float t = 0f;
        while (t < fade)
        {
            t += Time.deltaTime;
            grupo.alpha = Mathf.Lerp(desde, hasta, t / fade);
            yield return null;
        }
        grupo.alpha = hasta;
    }
}
