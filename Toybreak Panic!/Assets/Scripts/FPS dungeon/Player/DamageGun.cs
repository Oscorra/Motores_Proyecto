using System.Collections;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float damage = 5;
    public float range;

    [Header("Laser visual")]
    [Tooltip("Punto de salida del haz. Si se deja vacio usa el propio arma.")]
    public Transform muzzle;
    public Color colorLaser = new Color(0.2f, 1f, 1f, 1f);
    public float anchoLaser = 0.05f;
    public float duracionLaser = 0.05f;

    private Transform playerCamera;
    private LineRenderer laser;

    void Start()
    {
        playerCamera = Camera.main.transform;
        CrearLaser();
    }

    private void CrearLaser()
    {
        var go = new GameObject("LaserBeam");
        go.transform.SetParent(transform, false);
        laser = go.AddComponent<LineRenderer>();
        laser.useWorldSpace = true;
        laser.positionCount = 2;
        laser.startWidth = anchoLaser;
        laser.endWidth = anchoLaser;
        laser.numCapVertices = 4;
        laser.material = new Material(Shader.Find("Sprites/Default"));
        laser.startColor = colorLaser;
        laser.endColor = colorLaser;
        laser.enabled = false;
    }

    public void Shooting()
    {
        Ray gunray = new Ray(playerCamera.position, playerCamera.forward);
        Vector3 origen = muzzle != null ? muzzle.position : transform.position;
        Vector3 destino = playerCamera.position + playerCamera.forward * range;

        if (Physics.Raycast(gunray, out RaycastHit hit, range))
        {
            destino = hit.point;
            if (hit.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.health -= damage;
            }
        }

        StartCoroutine(MostrarLaser(origen, destino));
    }

    private IEnumerator MostrarLaser(Vector3 origen, Vector3 destino)
    {
        if (laser == null) yield break;
        laser.SetPosition(0, origen);
        laser.SetPosition(1, destino);
        laser.enabled = true;
        yield return new WaitForSeconds(duracionLaser);
        laser.enabled = false;
    }
}
