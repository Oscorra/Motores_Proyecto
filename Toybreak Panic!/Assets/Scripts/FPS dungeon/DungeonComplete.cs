using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Zona de fin del Dungeon. Al entrar el jugador: desbloquea el jetpack,
/// encola el mensaje de felicitacion y vuelve al selector de niveles.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DungeonComplete : MonoBehaviour
{
    public string tagJugador = "Player";
    public string escenaSelector = "Level Selector";

    [TextArea]
    public string mensaje = "Level 1 Completed, Here you have a Blaster, now you can aim and shoot, good luck!";

    private bool yaActivado;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaActivado) return;
        if (!other.CompareTag(tagJugador)) return;

        yaActivado = true;
        GameProgress.Instance.DesbloquearLaser();
        GameProgress.Instance.EncolarMensaje(mensaje);
        GameProgress.Instance.EncolarSpawn("PostDungeon");
        SceneManager.LoadScene(escenaSelector);
    }
}
