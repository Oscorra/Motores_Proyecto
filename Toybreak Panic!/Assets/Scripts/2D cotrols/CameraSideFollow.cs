using UnityEngine;

/// <summary>
/// Camara lateral estilo plataformas 2.5D. Sigue al objetivo en X (y opcionalmente Y)
/// manteniendo una profundidad fija en Z, para que la perspectiva parezca 2D.
/// </summary>
public class CameraSideFollow : MonoBehaviour
{
    public Transform objetivo;

    [Header("Offset respecto al objetivo")]
    public float offsetX = 0f;
    public float offsetY = 3f;
    public float distanciaZ = -12f;

    [Header("Suavizado")]
    public float suavizado = 5f;
    public bool seguirEnY = true;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(
            objetivo.position.x + offsetX,
            seguirEnY ? objetivo.position.y + offsetY : transform.position.y,
            distanciaZ);

        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
    }
}
