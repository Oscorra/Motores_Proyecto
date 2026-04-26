using UnityEngine;

public class ObjectMaterialSelector : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject[] objects;

    [Header("Materiales")]
    public Material[] materials;

    [Header("Visual de seleccion")]
    public float selectedYOffset = 0.25f;

    private int selectedObjectIndex = 0;
    private int selectedMaterialIndex = 0;
    private Vector3[] initialLocalPositions;
    private int previousSelectedObjectIndex = -1;

    public int SelectedObjectIndex => selectedObjectIndex;
    public int SelectedMaterialIndex => selectedMaterialIndex;
    public int ObjectCount => objects != null ? objects.Length : 0;
    public int MaterialCount => materials != null ? materials.Length : 0;

    public GameObject SelectedObject => GetSelectedObject();

    public Renderer SelectedRenderer
    {
        get
        {
            GameObject selectedObject = GetSelectedObject();
            return selectedObject != null ? selectedObject.GetComponent<Renderer>() : null;
        }
    }

    public Material SelectedMaterial
    {
        get
        {
            Renderer rend = SelectedRenderer;
            return rend != null ? rend.material : null;
        }
    }

    void Start()
    {
        CacheInitialPositions();
        UpdateSelectedObjectVisual();
    }

    void Update()
    {
        HandleSelectionInput();
        HandleMaterialInput();
    }

    private void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectObject(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectObject(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectObject(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectObject(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectObject(4);
    }

    private void HandleMaterialInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (objects == null || materials == null) return;
        if (objects.Length == 0 || materials.Length == 0) return;

        GameObject selectedObject = GetSelectedObject();
        if (selectedObject == null) return;

        selectedMaterialIndex++;
        if (selectedMaterialIndex >= materials.Length)
            selectedMaterialIndex = 0;

        Debug.Log("Objeto seleccionado: " + selectedObject.name + ". Material seleccionado:  " + materials[selectedMaterialIndex].name);

        Renderer rend = selectedObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = materials[selectedMaterialIndex];
        }
    }

    private void CacheInitialPositions()
    {
        if (objects == null)
        {
            initialLocalPositions = new Vector3[0];
            return;
        }

        initialLocalPositions = new Vector3[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                initialLocalPositions[i] = objects[i].transform.localPosition;
            }
        }
    }

    private void SelectObject(int newIndex)
    {
        if (objects == null || objects.Length == 0) return;
        if (newIndex < 0 || newIndex >= objects.Length) return;

        selectedObjectIndex = newIndex;
        UpdateSelectedObjectVisual();
    }

    private void UpdateSelectedObjectVisual()
    {
        if (objects == null || objects.Length == 0) return;

        if (initialLocalPositions == null || initialLocalPositions.Length != objects.Length)
        {
            CacheInitialPositions();
        }

        selectedObjectIndex = Mathf.Clamp(selectedObjectIndex, 0, objects.Length - 1);

        if (previousSelectedObjectIndex >= 0 && previousSelectedObjectIndex < objects.Length)
        {
            GameObject previousObject = objects[previousSelectedObjectIndex];
            if (previousObject != null)
            {
                previousObject.transform.localPosition = initialLocalPositions[previousSelectedObjectIndex];
            }
        }

        GameObject selectedObject = objects[selectedObjectIndex];
        if (selectedObject != null)
        {
            Vector3 selectedPosition = initialLocalPositions[selectedObjectIndex];
            selectedPosition.y += selectedYOffset;
            selectedObject.transform.localPosition = selectedPosition;
        }

        previousSelectedObjectIndex = selectedObjectIndex;
    }

    private GameObject GetSelectedObject()
    {
        if (objects == null || objects.Length == 0) return null;
        if (selectedObjectIndex < 0 || selectedObjectIndex >= objects.Length) return null;

        return objects[selectedObjectIndex];
    }
}
