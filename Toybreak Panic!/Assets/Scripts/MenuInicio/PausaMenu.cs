using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    public GameObject pausaPanel;
    private bool pausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
                Continuar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        pausaPanel.SetActive(true);
        Time.timeScale = 0f; // congela todo, incluidos enemigos
        pausado = true;
    }

    public void Continuar()
    {
        pausaPanel.SetActive(false);
        Time.timeScale = 1f; // reanuda todo
        pausado = false;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f; // importante resetear antes de cambiar escena
        SceneManager.LoadScene("MenuInicio");
    }
}