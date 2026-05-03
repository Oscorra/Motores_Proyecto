using UnityEngine;

public class ZonaControlParticulas : MonoBehaviour
{
    public ParticulasPies particulasPies;

    public float tasaEmisionMin = 5f;
    public float tasaEmisionMax = 80f;
    public float pasoEmision = 5f;

    private float tasaActual = 20f;
    private int indiceColor = 0;
    private bool jugadorDentro = false;

    private readonly Color[] coloresDisponibles = new Color[]
    {
        new Color(0.6f, 0.5f, 0.3f),
        new Color(0.8f, 0.3f, 0.1f),
        new Color(0.3f, 0.6f, 0.9f),
        new Color(0.9f, 0.9f, 0.2f),
        Color.white
    };

    private readonly string[] nombresColores = new string[]
    {
        "Arena", "Naranja", "Azul", "Amarillo", "Blanco"
    };

    void Update()
    {
        if (!jugadorDentro) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            tasaActual = Mathf.Max(tasaEmisionMin, tasaActual - pasoEmision);
            particulasPies?.SetEmisionRate(tasaActual);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            tasaActual = Mathf.Min(tasaEmisionMax, tasaActual + pasoEmision);
            particulasPies?.SetEmisionRate(tasaActual);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            indiceColor = (indiceColor + 1) % coloresDisponibles.Length;
            particulasPies?.SetColorInicio(coloresDisponibles[indiceColor]);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            bool activo = particulasPies != null && particulasPies.particulasPies != null
                          && particulasPies.particulasPies.isPlaying;
            particulasPies?.SetActivo(!activo);
        }
    }

    void OnGUI()
    {
        if (!jugadorDentro) return;

        float escala = Screen.height / 720f;
        float anchoPanel = 320f * escala;
        float altoFila = 36f * escala;
        float padding = 16f * escala;
        float margen = 20f * escala;
        int filas = 6;
        float altoPanel = altoFila * filas + padding * 2f;

        Rect panelRect = new Rect(margen, Screen.height - altoPanel - margen, anchoPanel, altoPanel);

        // Fondo semitransparente
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle estiloCabecera = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(15f * escala),
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(1f, 0.85f, 0.3f) }
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

        float x = panelRect.x + padding;
        float y = panelRect.y + padding;
        float anchoTexto = anchoPanel - padding * 2f;

        GUI.Label(new Rect(x, y, anchoTexto, altoFila), "ZONA DE CONTROL", estiloCabecera);
        y += altoFila;

        bool particActiva = particulasPies != null && particulasPies.particulasPies != null
                            && particulasPies.particulasPies.isPlaying;
        string estadoPartic = particActiva ? "ON" : "OFF";
        GUIStyle estiloEstado = new GUIStyle(estiloValor)
        {
            normal = { textColor = particActiva ? new Color(0.4f, 1f, 0.6f) : new Color(1f, 0.4f, 0.4f) }
        };

        DibujarFila(x, y, anchoTexto, altoFila, "[F]  Particulas:", estadoPartic, estiloFila, estiloEstado);
        y += altoFila;

        DibujarFila(x, y, anchoTexto, altoFila, "[Q/E]  Emision:", $"{tasaActual:0}", estiloFila, estiloValor);
        y += altoFila;

        DibujarFila(x, y, anchoTexto, altoFila, "[R]  Color:", nombresColores[indiceColor], estiloFila, estiloValor);
        y += altoFila;

        // Barra de progreso de emision
        float progreso = (tasaActual - tasaEmisionMin) / (tasaEmisionMax - tasaEmisionMin);
        float altoBarraPadding = 6f * escala;
        float altoBarra = altoFila - altoBarraPadding * 2f;
        Rect rectBarraFondo = new Rect(x, y + altoBarraPadding, anchoTexto, altoBarra);
        GUI.color = new Color(1f, 1f, 1f, 0.2f);
        GUI.DrawTexture(rectBarraFondo, Texture2D.whiteTexture);
        Rect rectBarraRelleno = new Rect(x, y + altoBarraPadding, anchoTexto * progreso, altoBarra);
        GUI.color = new Color(0.4f, 1f, 0.6f, 0.8f);
        GUI.DrawTexture(rectBarraRelleno, Texture2D.whiteTexture);
        GUI.color = Color.white;
        y += altoFila;

        // Muestra de color actual
        Rect rectColor = new Rect(x, y + altoBarraPadding, anchoTexto, altoBarra);
        GUI.color = coloresDisponibles[indiceColor];
        GUI.DrawTexture(rectColor, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DibujarFila(float x, float y, float ancho, float alto,
                              string etiqueta, string valor,
                              GUIStyle estiloEtiqueta, GUIStyle estiloValor)
    {
        float anchoEtiqueta = ancho * 0.65f;
        float anchoValor = ancho * 0.35f;
        GUI.Label(new Rect(x, y, anchoEtiqueta, alto), etiqueta, estiloEtiqueta);
        GUI.Label(new Rect(x + anchoEtiqueta, y, anchoValor, alto), valor, estiloValor);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        jugadorDentro = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        jugadorDentro = false;
    }
}
