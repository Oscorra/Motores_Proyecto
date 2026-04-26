using UnityEngine;
using UnityEngine.Rendering;

public class TogglePostProcess : MonoBehaviour
{
    public Volume volume;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            volume.enabled = !volume.enabled;
        }
    }
}
