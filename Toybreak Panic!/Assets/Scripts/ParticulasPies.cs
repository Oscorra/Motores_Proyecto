using UnityEngine;

public class ParticulasPies : MonoBehaviour
{
    public ParticleSystem particulasPies;

    void Start()
    {
        if (particulasPies == null) return;

        var renderer = particulasPies.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));

        var main = particulasPies.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.4f);
        main.startColor = new Color(0.6f, 0.5f, 0.3f, 0.8f);
        main.gravityModifier = 0.08f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particulasPies.emission;
        emission.rateOverTime = 10f;

        var shape = particulasPies.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(1.2f, 0.05f, 0.6f);

        var col = particulasPies.colorOverLifetime;
        col.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.6f, 0.5f, 0.3f), 0.0f),
                new GradientColorKey(new Color(0.7f, 0.6f, 0.5f), 0.5f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.8f, 0.0f),
                new GradientAlphaKey(0.4f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(gradient);

        var sol = particulasPies.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.3f);
        curve.AddKey(0.3f, 1.0f);
        curve.AddKey(1.0f, 0.0f);
        sol.size = new ParticleSystem.MinMaxCurve(1f, curve);

        particulasPies.Play();
    }

    public void SetEmisionRate(float rate)
    {
        if (particulasPies == null) return;
        var emission = particulasPies.emission;
        emission.rateOverTimeMultiplier = rate;
    }

    public void SetColorInicio(Color color)
    {
        if (particulasPies == null) return;
        var main = particulasPies.main;
        main.startColor = color;
    }

    public void SetActivo(bool activo)
    {
        if (particulasPies == null) return;
        if (activo && !particulasPies.isPlaying)
            particulasPies.Play();
        else if (!activo && particulasPies.isPlaying)
            particulasPies.Stop();
    }
}
