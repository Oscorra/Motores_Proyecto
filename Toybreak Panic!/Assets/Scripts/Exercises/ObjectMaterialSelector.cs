using UnityEngine;

public class ObjectMaterialSelector : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject[] objects;

    [Header("Materiales")]
    public Material[] materials;

    private int selectedObjectIndex = 0;
    private int selectedMaterialIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedObjectIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedObjectIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedObjectIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedObjectIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedObjectIndex = 4;

        if (Input.GetMouseButtonDown(0))
        {
            if (objects.Length == 0 || materials.Length == 0) return;
            if (selectedObjectIndex < 0 || selectedObjectIndex >= objects.Length) return;

            selectedMaterialIndex++;
            if (selectedMaterialIndex >= materials.Length)
                selectedMaterialIndex = 0;

            Debug.Log("Objeto seleccionado: " + objects[selectedObjectIndex].name + ". Material seleccionado:  " + materials[selectedMaterialIndex].name);

            Renderer rend = objects[selectedObjectIndex].GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = materials[selectedMaterialIndex];
            }
        }
    }
}
