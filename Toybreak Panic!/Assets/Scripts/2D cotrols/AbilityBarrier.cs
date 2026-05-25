using UnityEngine;

/// <summary>
/// Obstaculo gateado por habilidad. Mientras el jugador NO tenga la habilidad requerida,
/// la barrera permanece activa (bloquea el paso). Al desbloquearla, se aparta/desactiva.
/// </summary>
public class AbilityBarrier : MonoBehaviour
{
    public enum Habilidad { Laser, Jetpack }

    [Tooltip("Que habilidad hace falta para superar/abrir esta barrera.")]
    public Habilidad habilidadRequerida = Habilidad.Laser;

    [Tooltip("Objeto a desactivar cuando se desbloquea (por defecto, este mismo).")]
    public GameObject objetoABloquear;

    private void OnEnable()
    {
        if (objetoABloquear == null) objetoABloquear = gameObject;
        Actualizar();
    }

    private void Update()
    {
        // La barrera puede desbloquearse al volver de un nivel, asi que comprobamos en vivo.
        if (objetoABloquear != null && objetoABloquear.activeSelf)
            Actualizar();
    }

    private void Actualizar()
    {
        bool desbloqueada = habilidadRequerida == Habilidad.Laser
            ? GameProgress.Instance.TieneLaser
            : GameProgress.Instance.TieneJetpack;

        if (desbloqueada && objetoABloquear.activeSelf)
            objetoABloquear.SetActive(false);
    }
}
