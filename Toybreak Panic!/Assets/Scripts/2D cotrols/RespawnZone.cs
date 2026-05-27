using UnityEngine;

/// <summary>
/// Volumen de caida bajo el hueco de las lianas. Si el jugador cae dentro,
/// reaparece en el punto indicado (justo despues del muro) con la velocidad
/// reseteada y cancelando cualquier balanceo en curso.
/// </summary>
[RequireComponent(typeof(Collider))]
public class RespawnZone : MonoBehaviour
{
    public Transform puntoRespawn;
    public string tagJugador = "Player";

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        Transform root = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;

        var swing = root.GetComponent<PlayerSwing>();
        if (swing != null) swing.Cancelar();

        var rb = root.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (puntoRespawn != null)
            root.SetPositionAndRotation(puntoRespawn.position, puntoRespawn.rotation);
    }
}
