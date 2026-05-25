using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// "Caja de juguete": al entrar el jugador en el trigger, carga la escena del nivel.
/// Las escenas deben estar añadidas en Build Settings.
/// </summary>
[RequireComponent(typeof(Collider))]
public class LevelEntrance : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena a cargar (debe estar en Build Settings).")]
    public string nombreEscena = "Dungeon";

    [Tooltip("Tag del jugador que activa la entrada.")]
    public string tagJugador = "Player";

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("[LevelEntrance] No hay nombre de escena asignado.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
        else
            Debug.LogWarning($"[LevelEntrance] La escena '{nombreEscena}' no esta en Build Settings.");
    }
}
