using UnityEngine;

public class FogZone : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = Color.gray;
    public FogMode fogMode = FogMode.ExponentialSquared;
    public float fogDensity = 0.03f;

    [Header("Runtime Control")]
    public KeyCode toggleFogKey = KeyCode.F;
    public KeyCode increaseDensityKey = KeyCode.UpArrow;
    public KeyCode decreaseDensityKey = KeyCode.DownArrow;
    public float densityStep = 0.005f;

    private bool playerInside = false;

    private bool previousFogState;
    private Color previousFogColor;
    private FogMode previousFogMode;
    private float previousFogDensity;

    void Start()
    {
        SavePreviousFogSettings();
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(toggleFogKey))
        {
            RenderSettings.fog = !RenderSettings.fog;
        }

        if (Input.GetKeyDown(increaseDensityKey))
        {
            fogDensity += densityStep;
            ApplyFogSettings();
        }

        if (Input.GetKeyDown(decreaseDensityKey))
        {
            fogDensity -= densityStep;

            if (fogDensity < 0f)
                fogDensity = 0f;

            ApplyFogSettings();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        SavePreviousFogSettings();
        ApplyFogSettings();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        RestorePreviousFogSettings();
    }

    void ApplyFogSettings()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    void SavePreviousFogSettings()
    {
        previousFogState = RenderSettings.fog;
        previousFogColor = RenderSettings.fogColor;
        previousFogMode = RenderSettings.fogMode;
        previousFogDensity = RenderSettings.fogDensity;
    }

    void RestorePreviousFogSettings()
    {
        RenderSettings.fog = previousFogState;
        RenderSettings.fogColor = previousFogColor;
        RenderSettings.fogMode = previousFogMode;
        RenderSettings.fogDensity = previousFogDensity;
    }

    void OnGUI()
    {
        if (!playerInside) return;

        float escala = Screen.height / 720f;
        float anchoPanel = 320f * escala;
        float altoFila = 36f * escala;
        float padding = 16f * escala;
        float margen = 20f * escala;
        int filas = 6;
        float altoPanel = altoFila * filas + padding * 2f;

        float panelX = Screen.width - anchoPanel - margen;
        float panelY = margen;
        Rect panelRect = new Rect(panelX, panelY, anchoPanel, altoPanel);

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle estiloCabecera = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(15f * escala),
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.7f, 0.85f, 1f) }
        };

        GUIStyle estiloFila = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(13f * escala),
            normal = { textColor = Color.white }
        };

        GUIStyle estiloValor = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(13f * escala),
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.4f, 1f, 0.6f) }
        };

        float cx = panelRect.x + padding;
        float cy = panelRect.y + padding;
        float anchoTexto = anchoPanel - padding * 2f;

        GUI.Label(new Rect(cx, cy, anchoTexto, altoFila), "ZONA DE NIEBLA", estiloCabecera);
        cy += altoFila;

        bool fogActivo = RenderSettings.fog;
        GUIStyle estiloEstado = new GUIStyle(estiloValor)
        {
            normal = { textColor = fogActivo ? new Color(0.4f, 1f, 0.6f) : new Color(1f, 0.4f, 0.4f) }
        };
        DibujarFila(cx, cy, anchoTexto, altoFila, $"[{toggleFogKey}]  Niebla:", fogActivo ? "ON" : "OFF", estiloFila, estiloEstado);
        cy += altoFila;

        DibujarFila(cx, cy, anchoTexto, altoFila, $"[{increaseDensityKey}/{decreaseDensityKey}]  Densidad:", $"{fogDensity:F3}", estiloFila, estiloValor);
        cy += altoFila;

        DibujarFila(cx, cy, anchoTexto, altoFila, "Modo:", fogMode.ToString(), estiloFila, estiloValor);
        cy += altoFila;

        // Barra de densidad (0 a 0.1 como referencia visual)
        float progreso = Mathf.Clamp01(fogDensity / 0.1f);
        float altoBarraPadding = 6f * escala;
        float altoBarra = altoFila - altoBarraPadding * 2f;
        GUI.color = new Color(1f, 1f, 1f, 0.2f);
        GUI.DrawTexture(new Rect(cx, cy + altoBarraPadding, anchoTexto, altoBarra), Texture2D.whiteTexture);
        GUI.color = new Color(0.7f, 0.85f, 1f, 0.8f);
        GUI.DrawTexture(new Rect(cx, cy + altoBarraPadding, anchoTexto * progreso, altoBarra), Texture2D.whiteTexture);
        GUI.color = Color.white;
        cy += altoFila;

        // Muestra del color de niebla
        float anchoEtiqueta = anchoTexto * 0.55f;
        GUI.Label(new Rect(cx, cy, anchoEtiqueta, altoFila), "Color niebla:", estiloFila);
        GUI.color = fogColor;
        GUI.DrawTexture(new Rect(cx + anchoEtiqueta, cy + altoBarraPadding, anchoTexto - anchoEtiqueta, altoBarra), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DibujarFila(float x, float y, float ancho, float alto,
                              string etiqueta, string valor,
                              GUIStyle estiloEtiqueta, GUIStyle estiloValor)
    {
        float anchoEtiqueta = ancho * 0.65f;
        GUI.Label(new Rect(x, y, anchoEtiqueta, alto), etiqueta, estiloEtiqueta);
        GUI.Label(new Rect(x + anchoEtiqueta, y, ancho - anchoEtiqueta, alto), valor, estiloValor);
    }
}