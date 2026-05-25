using UnityEngine;

/// <summary>
/// Progresion persistente del jugador. Sobrevive a los cambios de escena
/// (lobby 2.5D, dungeon FPS, parkour). Guarda que habilidades estan desbloqueadas.
/// Se auto-crea la primera vez que se accede, asi funciona arrancando desde cualquier escena.
/// </summary>
public class GameProgress : MonoBehaviour
{
    private static GameProgress _instance;

    public static GameProgress Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameProgress>();
                if (_instance == null)
                {
                    var go = new GameObject("GameProgress");
                    _instance = go.AddComponent<GameProgress>();
                }
            }
            return _instance;
        }
    }

    public bool TieneLaser { get; private set; }
    public bool TieneJetpack { get; private set; }

    /// <summary>Mensaje a mostrar tras cargar la siguiente escena (p.ej. al volver al selector).</summary>
    public string MensajePendiente { get; private set; }

    /// <summary>Id del punto donde debe reaparecer el jugador al cargar la siguiente escena.</summary>
    public string ProximoSpawn { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DesbloquearLaser()
    {
        TieneLaser = true;
        Debug.Log("[GameProgress] Laser desbloqueado.");
    }

    public void DesbloquearJetpack()
    {
        TieneJetpack = true;
        Debug.Log("[GameProgress] Jetpack desbloqueado.");
    }

    /// <summary>Encola un mensaje para mostrarlo en la siguiente escena.</summary>
    public void EncolarMensaje(string mensaje)
    {
        MensajePendiente = mensaje;
    }

    /// <summary>Devuelve el mensaje pendiente y lo limpia (se muestra una sola vez).</summary>
    public string ConsumirMensaje()
    {
        string m = MensajePendiente;
        MensajePendiente = null;
        return m;
    }

    /// <summary>Encola el punto de reaparicion para la siguiente escena.</summary>
    public void EncolarSpawn(string id)
    {
        ProximoSpawn = id;
    }

    /// <summary>Devuelve el punto de reaparicion pendiente y lo limpia.</summary>
    public string ConsumirSpawn()
    {
        string s = ProximoSpawn;
        ProximoSpawn = null;
        return s;
    }

    /// <summary>Reinicia el progreso (util para empezar la demo de cero).</summary>
    public void Reiniciar()
    {
        TieneLaser = false;
        TieneJetpack = false;
        MensajePendiente = null;
        ProximoSpawn = null;
    }
}
