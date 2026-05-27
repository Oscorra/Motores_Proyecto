using UnityEngine;

/// <summary>
/// Arranque del nivel Parkour. Garantiza que el jugador disponga del jetpack
/// (que en el flujo normal ya se recoge en el selector antes de entrar aqui),
/// para que el nivel sea jugable tambien si se carga directamente.
/// </summary>
public class ParkourSetup : MonoBehaviour
{
    void Awake()
    {
        GameProgress.Instance.DesbloquearJetpack();
    }
}
