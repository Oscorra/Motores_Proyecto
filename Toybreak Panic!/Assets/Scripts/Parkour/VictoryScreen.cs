using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Pantalla de felicitacion al completar el parkour. Cablea automaticamente
/// los botones hijos (por nombre) para no depender de UnityEvents en el editor.
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    public string escenaMenu = "MenuInicio";

    void Awake()
    {
        foreach (var b in GetComponentsInChildren<Button>(true))
        {
            string n = b.gameObject.name;
            if (n == "BtnMenu") b.onClick.AddListener(IrAlMenu);
            else if (n == "BtnSalir") b.onClick.AddListener(Salir);
        }
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
