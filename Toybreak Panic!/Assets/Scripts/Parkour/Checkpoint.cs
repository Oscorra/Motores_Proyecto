using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 ultimoCheckpoint;
    private bool activado = false;

    void Start()
    {
        if (ultimoCheckpoint == Vector3.zero)
        {
            GameObject jugador = GameObject.FindWithTag("Player");
            if (jugador != null)
                ultimoCheckpoint = jugador.transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            ultimoCheckpoint = transform.position + Vector3.up * 1f;
            Debug.Log("Checkpoint activado: " + transform.position);
            // Aquí puedes cambiar el color del checkpoint para indicar que está activado
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}