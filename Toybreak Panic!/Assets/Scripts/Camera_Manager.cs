using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    [Header("Cámara a controlar")]
    public Camera cam;

    [Header("Ajustes")]
    public float fovStep = 5f;
    public float clipStep = 0.1f;

    private void Update()
    {
        if (cam == null) return;

        //fov
        if (Input.GetKeyDown(KeyCode.Z))
            cam.fieldOfView -= fovStep;

        if (Input.GetKeyDown(KeyCode.X))
            cam.fieldOfView += fovStep;

        //clipping planes
        if (Input.GetKeyDown(KeyCode.C))
            cam.nearClipPlane = Mathf.Max(0.01f, cam.nearClipPlane - clipStep);

        if (Input.GetKeyDown(KeyCode.V))
            cam.nearClipPlane += clipStep;

        if (Input.GetKeyDown(KeyCode.B))
            cam.farClipPlane -= clipStep * 10f;

        if (Input.GetKeyDown(KeyCode.N))
            cam.farClipPlane += clipStep * 10f;

        //perspectiva
        if (Input.GetKeyDown(KeyCode.M))
            cam.orthographic = !cam.orthographic;

        if (cam.orthographic)
        {
            if (Input.GetKeyDown(KeyCode.Z))
                cam.orthographicSize -= 0.5f;

            if (Input.GetKeyDown(KeyCode.X))
                cam.orthographicSize += 0.5f;
        }
    }
}
