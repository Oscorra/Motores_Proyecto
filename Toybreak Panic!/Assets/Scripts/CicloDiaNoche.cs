using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    public Light luzDireccional;
    public float velocidadRotacion = 10f;

    private bool cicloActivo = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            cicloActivo = !cicloActivo;

        if (cicloActivo)
            luzDireccional.transform.Rotate(Vector3.right, velocidadRotacion * Time.deltaTime);
    }
}
