using UnityEngine;

public class CustomShader : MonoBehaviour
{
    [Header("Objetivo")]
    public Renderer targetRenderer;
    public Shader customShader;

    [Header("Propiedades del shader")]
    public Color shaderColor = new Color(0.2f, 0.8f, 1f, 1f);
    [Range(0f, 5f)] public float changeSpeed = 1f;

    private Material runtimeMaterial;

    void Reset()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        ApplyShader();
    }

    void Update()
    {
        UpdateShaderProperties();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;

        ApplyShader();
    }

    void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }

    private void ApplyShader()
    {
        if (targetRenderer == null || customShader == null) return;

        if (runtimeMaterial == null || runtimeMaterial.shader != customShader)
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }

            runtimeMaterial = new Material(customShader);
            runtimeMaterial.name = customShader.name + " Instance";
            targetRenderer.material = runtimeMaterial;
        }

        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        if (runtimeMaterial == null) return;

        runtimeMaterial.SetColor("_Color", shaderColor);
        runtimeMaterial.SetFloat("_Speed", changeSpeed);
    }
}
