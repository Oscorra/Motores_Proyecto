using UnityEngine;

public class Camera_Bonus : MonoBehaviour
{
    [Header("Camara")]
    public Camera cam;
    public Transform objetivo;

    [Header("Distancia para las conversiones")]
    public float distancia = 10f;

    [Header("Viewport")]
    [Range(0f, 1f)] public float viewportX = 0.5f;
    [Range(0f, 1f)] public float viewportY = 0.5f;

    [Header("Marcadores")]
    public GameObject marcadorRaton;
    public GameObject marcadorViewport;

    [Header("Valores visibles en el Inspector")]
    public Vector3 posicionPantallaObjetivo;
    public Vector3 posicionPantallaRaton;
    public Vector3 posicionMundoRaton;
    public Vector3 posicionMundoViewport;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (objetivo == null)
        {
            Player_Movement jugador = FindObjectOfType<Player_Movement>();

            if (jugador != null)
            {
                objetivo = jugador.transform;
            }
        }

        if (marcadorRaton == null)
        {
            marcadorRaton = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marcadorRaton.name = "Marcador_Raton";
            marcadorRaton.transform.localScale = Vector3.one * 0.5f;
            marcadorRaton.GetComponent<Renderer>().material.color = Color.red;
            marcadorRaton.GetComponent<Collider>().enabled = false;
        }

        if (marcadorViewport == null)
        {
            marcadorViewport = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marcadorViewport.name = "Marcador_Viewport";
            marcadorViewport.transform.localScale = Vector3.one * 0.5f;
            marcadorViewport.GetComponent<Renderer>().material.color = Color.cyan;
            marcadorViewport.GetComponent<Collider>().enabled = false;
        }
    }

    void Update()
    {
        if (cam == null)
        {
            return;
        }

        if (objetivo != null)
        {
            posicionPantallaObjetivo = cam.WorldToScreenPoint(objetivo.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            posicionPantallaRaton = Input.mousePosition;
            posicionPantallaRaton.z = distancia;
            posicionMundoRaton = cam.ScreenToWorldPoint(posicionPantallaRaton);

            if (marcadorRaton != null)
            {
                marcadorRaton.transform.position = posicionMundoRaton;
            }

            Debug.Log("ScreenToWorldPoint: " + posicionPantallaRaton + " -> " + posicionMundoRaton);
        }

        Vector3 puntoViewport = new Vector3(viewportX, viewportY, distancia);
        posicionMundoViewport = cam.ViewportToWorldPoint(puntoViewport);

        if (marcadorViewport != null)
        {
            marcadorViewport.transform.position = posicionMundoViewport;
        }
    }
}
