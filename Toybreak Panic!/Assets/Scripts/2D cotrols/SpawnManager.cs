using UnityEngine;

/// <summary>
/// Reposiciona al jugador segun el punto de reaparicion encolado en GameProgress.
/// Al volver del Dungeon ("PostDungeon"), aparece despues de la pared roja.
/// Si no hay punto encolado (primer arranque), el jugador se queda donde este en la escena.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public Transform jugador;
    public Transform puntoPostDungeon;

    private void Start()
    {
        string id = GameProgress.Instance.ConsumirSpawn();
        if (string.IsNullOrEmpty(id) || jugador == null) return;

        Transform destino = id == "PostDungeon" ? puntoPostDungeon : null;
        if (destino == null) return;

        jugador.SetPositionAndRotation(destino.position, destino.rotation);

        var rb = jugador.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
