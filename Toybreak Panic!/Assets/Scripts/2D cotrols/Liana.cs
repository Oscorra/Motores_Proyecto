using UnityEngine;

/// <summary>
/// Liana colgante para balanceo tipo pendulo. El pivote es la posicion de este
/// transform (parte alta). El cuerpo cuelga hacia abajo como hijo; PlayerSwing
/// hace rotar la liana alrededor del pivote (eje Z) mientras el jugador se balancea.
/// Lleva un collider trigger que cubre la zona de agarre.
/// </summary>
public class Liana : MonoBehaviour
{
    [Tooltip("Longitud del pivote al punto de agarre (donde cuelga el jugador).")]
        public float largo = 4f;

    [Header("Balanceo en reposo (cuando nadie esta colgado)")]
    [Tooltip("Amplitud del balanceo idle en grados.")]
    public float amplitudIdle = 22f;
    [Tooltip("Velocidad del balanceo idle.")]
    public float velocidadIdle = 1.6f;
    [Tooltip("Desfase para que dos lianas vayan en sentidos opuestos (0 y PI).")]
    public float fase = 0f;

    private bool controlada;

    /// <summary>Punto fijo (arriba) alrededor del que oscila la liana.</summary>
        public Vector3 Pivote { get { return transform.position; } }

    void Update()
    {
        if (controlada) return;
        float ang = amplitudIdle * Mathf.Deg2Rad * Mathf.Sin(Time.time * velocidadIdle + fase);
        SetAngulo(ang);
    }

    /// <summary>PlayerSwing toma el control del angulo mientras el jugador cuelga.</summary>
    public void TomarControl() { controlada = true; }
    /// <summary>Devuelve la liana a su balanceo automatico.</summary>
    public void SoltarControl() { controlada = false; }

    /// <summary>Orienta la liana segun el angulo (radianes) desde la vertical hacia abajo.</summary>
    public void SetAngulo(float anguloRad)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, anguloRad * Mathf.Rad2Deg);
    }
}
