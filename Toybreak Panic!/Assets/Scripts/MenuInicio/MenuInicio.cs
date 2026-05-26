using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    public GameObject controlesPanel;

    public void Jugar()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void MostrarControles()
    {
        controlesPanel.SetActive(true);
    }

    public void OcultarControles()
    {
        controlesPanel.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
    }
}