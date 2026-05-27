using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Meta del nivel Parkour. Al entrar el jugador muestra la pantalla de
/// felicitacion (estilo menu de inicio), congela el juego y libera el cursor.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ParkourComplete : MonoBehaviour
{
    public GameObject pantallaVictoria;
    public string tagJugador = "Player";

    private bool completado;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (completado) return;
        if (!other.CompareTag(tagJugador)) return;

        completado = true;
        if (pantallaVictoria != null) pantallaVictoria.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicio");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
