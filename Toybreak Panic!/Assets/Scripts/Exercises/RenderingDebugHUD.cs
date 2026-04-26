using UnityEngine;

public class RenderingDebugHUD : MonoBehaviour
{
    [Header("Referencias")]
    public ObjectMaterialSelector selector;
    public SelectedObjectColorAnimator colorAnimator;

    [Header("Debug HUD")]
    public bool showDebugHUD = true;

    private GUIStyle hudBoxStyle;
    private GUIStyle hudTitleStyle;
    private GUIStyle hudSectionStyle;
    private GUIStyle hudLabelStyle;

    void Start()
    {
        if (selector == null)
        {
            selector = FindObjectOfType<ObjectMaterialSelector>();
        }

        if (colorAnimator == null)
        {
            colorAnimator = FindObjectOfType<SelectedObjectColorAnimator>();
        }

    }

    void OnGUI()
    {
        if (!showDebugHUD) return;

        InitializeHUDStyles();

        Rect hudRect = GetHUDRect();
        GUILayout.BeginArea(hudRect, GUIContent.none, hudBoxStyle);

        GUILayout.Label("Debug HUD", hudTitleStyle);
        if (ShouldUseTwoColumnHUD(hudRect))
        {
            float columnWidth = (hudRect.width - hudBoxStyle.padding.horizontal - GetHUDSpacing()) * 0.5f;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(columnWidth));
            DrawSelectedObjectInfo();
            GUILayout.EndVertical();

            GUILayout.Space(GetHUDSpacing());

            GUILayout.BeginVertical(GUILayout.Width(columnWidth));
            DrawControlsInfo();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        else
        {
            DrawSelectedObjectInfo();
            GUILayout.Space(GetHUDSpacing());
            DrawControlsInfo();
        }

        GUILayout.EndArea();
    }

    private void InitializeHUDStyles()
    {
        if (hudBoxStyle == null)
        {
            hudBoxStyle = new GUIStyle(GUI.skin.box);
            hudTitleStyle = new GUIStyle(GUI.skin.label);
            hudSectionStyle = new GUIStyle(GUI.skin.label);
            hudLabelStyle = new GUIStyle(GUI.skin.label);
        }

        hudBoxStyle.alignment = TextAnchor.UpperLeft;
        int padding = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.012f), 6, 12);
        hudBoxStyle.padding = new RectOffset(padding, padding, padding, padding);

        hudTitleStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.024f), 9, 15);
        hudTitleStyle.fontStyle = FontStyle.Bold;
        hudTitleStyle.margin = new RectOffset(0, 0, 0, 4);
        hudTitleStyle.normal.textColor = Color.white;

        hudSectionStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.02f), 8, 12);
        hudSectionStyle.fontStyle = FontStyle.Bold;
        hudSectionStyle.margin = new RectOffset(0, 0, 2, 1);
        hudSectionStyle.normal.textColor = new Color(1f, 0.9f, 0.5f);

        hudLabelStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.016f), 7, 11);
        hudLabelStyle.margin = new RectOffset(0, 0, 0, 0);
        hudLabelStyle.wordWrap = false;
        hudLabelStyle.clipping = TextClipping.Clip;
        hudLabelStyle.normal.textColor = Color.white;
    }

    private Rect GetHUDRect()
    {
        float margin = Mathf.Clamp(Screen.width * 0.015f, 6f, 16f);
        float minWidth = Mathf.Min(220f, Screen.width * 0.55f);
        float width = Mathf.Clamp(Screen.width * 0.36f, minWidth, 520f);
        float minHeight = Mathf.Min(180f, Screen.height * 0.4f);
        float height = Mathf.Clamp(Screen.height * 0.48f, minHeight, 420f);

        width = Mathf.Min(width, Screen.width - margin * 2f);
        height = Mathf.Min(height, Screen.height - margin * 2f);

        return new Rect(margin, margin, width, height);
    }

    private float GetHUDSpacing()
    {
        return Mathf.Clamp(Screen.height * 0.008f, 3f, 8f);
    }

    private bool ShouldUseTwoColumnHUD(Rect hudRect)
    {
        return hudRect.width >= 320f;
    }

    private void DrawSelectedObjectInfo()
    {
        GUILayout.Label("Material actual", hudSectionStyle);

        if (selector == null)
        {
            GUILayout.Label("Selector no asignado.", hudLabelStyle);
            return;
        }

        GameObject selectedObject = selector.SelectedObject;
        if (selectedObject == null)
        {
            GUILayout.Label("No hay objeto seleccionado.", hudLabelStyle);
            return;
        }

        Material selectedMaterial = selector.SelectedMaterial;
        if (selectedMaterial == null)
        {
            GUILayout.Label("El objeto seleccionado no tiene Material.", hudLabelStyle);
            return;
        }

        int materialDisplayIndex = selector.MaterialCount > 0 ? selector.SelectedMaterialIndex + 1 : 0;

        GUILayout.Label("Obj " + (selector.SelectedObjectIndex + 1) + "/" + selector.ObjectCount + ": " + TruncateForHUD(selectedObject.name, 18), hudLabelStyle);
        GUILayout.Label("Mat " + materialDisplayIndex + "/" + selector.MaterialCount + ": " + TruncateForHUD(selectedMaterial.name, 18), hudLabelStyle);
        GUILayout.Label("Shader: " + TruncateForHUD(selectedMaterial.shader.name, 20), hudLabelStyle);

        Color currentColor;
        if (TryGetColorProperty(selectedMaterial, out currentColor))
        {
            GUILayout.Label(
                string.Format(
                    "RGBA: {0:F2} {1:F2} {2:F2} {3:F2}",
                    currentColor.r,
                    currentColor.g,
                    currentColor.b,
                    currentColor.a),
                hudLabelStyle);
        }
        else
        {
            GUILayout.Label("Color: N/A", hudLabelStyle);
        }

        GUILayout.Label(
            GetFloatPropertyText(selectedMaterial, "Met", "_Metallic") + " | " +
            GetFloatPropertyText(selectedMaterial, "Smooth", "_Glossiness", "_Smoothness"),
            hudLabelStyle);

        if (colorAnimator != null)
        {
            GUILayout.Label("Lerp: " + (colorAnimator.IsAnimationRunning ? "ON" : "OFF") + " | " + colorAnimator.lerpDuration.ToString("F1") + "s", hudLabelStyle);
        }
    }

    private void DrawControlsInfo()
    {
        GUILayout.Label("Controles", hudSectionStyle);
        GUILayout.Label("1-5 objeto | LMB material", hudLabelStyle);
        GUILayout.Label("Space: activar Color.Lerp", hudLabelStyle);
           
    }

    private bool TryGetColorProperty(Material material, out Color colorValue)
    {
        colorValue = Color.white;
        if (material == null) return false;

        if (material.HasProperty("_Color"))
        {
            colorValue = material.color;
            return true;
        }

        if (material.HasProperty("_BaseColor"))
        {
            colorValue = material.GetColor("_BaseColor");
            return true;
        }

        return false;
    }

    private string GetFloatPropertyText(Material material, string label, params string[] propertyNames)
    {
        float value;
        if (TryGetFloatProperty(material, out value, propertyNames))
        {
            return label + ": " + value.ToString("F2");
        }

        return label + ": N/A";
    }

    private bool TryGetFloatProperty(Material material, out float value, params string[] propertyNames)
    {
        value = 0f;
        if (material == null) return false;

        for (int i = 0; i < propertyNames.Length; i++)
        {
            if (material.HasProperty(propertyNames[i]))
            {
                value = material.GetFloat(propertyNames[i]);
                return true;
            }
        }

        return false;
    }

    private string TruncateForHUD(string text, int maxChars)
    {
        if (string.IsNullOrEmpty(text)) return "-";
        if (text.Length <= maxChars) return text;

        return text.Substring(0, maxChars - 3) + "...";
    }
}
