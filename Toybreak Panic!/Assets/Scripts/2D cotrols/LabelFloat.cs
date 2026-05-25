using UnityEngine;

/// <summary>
/// Anima la etiqueta de un portal: orbita alrededor del haz (eje vertical del padre),
/// con un ligero bote vertical. Mantiene el texto mirando a +Z (la direccion de la
/// camara lateral) para que siga siendo legible mientras gira.
/// </summary>
public class LabelFloat : MonoBehaviour
{
    [Tooltip("Radio de la orbita alrededor del haz.")]
    public float radio = 1.3f;

    [Tooltip("Velocidad de giro en grados por segundo.")]
    public float velocidadGiroGrados = 45f;

    [Tooltip("Amplitud del bote vertical.")]
    public float alturaBote = 0.15f;

    [Tooltip("Velocidad del bote vertical.")]
    public float velocidadBote = 2.5f;

    private float baseY;
    private float angulo;

    private void Start()
    {
        baseY = transform.localPosition.y;
    }

    private void Update()
    {
        angulo += velocidadGiroGrados * Mathf.Deg2Rad * Time.deltaTime;

        float x = Mathf.Cos(angulo) * radio;
        float z = Mathf.Sin(angulo) * radio;
        float y = baseY + Mathf.Sin(Time.time * velocidadBote) * alturaBote;

        transform.localPosition = new Vector3(x, y, z);
        transform.localRotation = Quaternion.identity;
    }
}
