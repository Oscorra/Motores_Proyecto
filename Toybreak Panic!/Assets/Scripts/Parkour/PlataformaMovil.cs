using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public float tiempoEspera = 1f;

    private Vector3 posicionAnterior;
    private Vector3 velocidadActual;
    private bool yendo = true;
    private bool esperando = false;
    private float timerEspera = 0f;

    void Start()
    {
        posicionAnterior = transform.position;
        transform.position = puntoA.position;
    }

    void Update()
    {
        if (esperando)
        {
            timerEspera += Time.deltaTime;
            if (timerEspera >= tiempoEspera)
            {
                esperando = false;
                timerEspera = 0f;
                yendo = !yendo;
            }
            return;
        }

        Vector3 destino = yendo ? puntoB.position : puntoA.position;
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            esperando = true;
        }

        velocidadActual = transform.position - posicionAnterior;
        posicionAnterior = transform.position;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.controller != null)
        {
            hit.controller.Move(velocidadActual);
        }
    }
}