using UnityEngine;

/// <summary>
/// Mantiene un objeto OCULTO hasta que el jugador desbloquea la habilidad requerida,
/// momento en el que lo revela. Pensado para portales de nivel que no deben verse
/// (ni poder entrarse) hasta superar el nivel anterior.
///
/// IMPORTANTE: este componente debe ir en un GameObject SIEMPRE ACTIVO distinto del
/// objeto que controla, porque un objeto desactivado no ejecuta su propio Update.
/// </summary>
public class RevealOnAbility : MonoBehaviour
{
    public enum Habilidad { Laser, Jetpack }

    [Tooltip("Habilidad que hace falta para revelar el objeto.")]
    public Habilidad habilidadRequerida = Habilidad.Jetpack;

    [Tooltip("Objeto a revelar/ocultar (el portal).")]
    public GameObject objetoARevelar;

    private void Update()
    {
        if (objetoARevelar == null) return;

        bool desbloqueada = habilidadRequerida == Habilidad.Jetpack
            ? GameProgress.Instance.TieneJetpack
            : GameProgress.Instance.TieneLaser;

        if (objetoARevelar.activeSelf != desbloqueada)
            objetoARevelar.SetActive(desbloqueada);
    }
}
