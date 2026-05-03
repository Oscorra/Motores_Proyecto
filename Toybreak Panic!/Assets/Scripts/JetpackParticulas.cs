using UnityEngine;

public class JetpackParticulas : MonoBehaviour
{
    public ParticleSystem particulasJetpack;

    void Start()
    {
        if (particulasJetpack == null) return;

        var renderer = particulasJetpack.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));

        var main = particulasJetpack.main;
        main.loop = true;
        main.startLifetime = 0.4f;
        main.startSpeed = 1.0f;
        main.startSize = 0.2f;
        main.startColor = Color.yellow;
        main.gravityModifier = 0.3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particulasJetpack.emission;
        emission.rateOverTime = 40f;

        var shape = particulasJetpack.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.05f;

        var col = particulasJetpack.colorOverLifetime;
        col.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.yellow, 0.0f),
                new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f),
                new GradientColorKey(Color.red, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.6f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(gradient);

        var sol = particulasJetpack.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.2f);
        curve.AddKey(0.3f, 1.0f);
        curve.AddKey(1.0f, 0.0f);
        sol.size = new ParticleSystem.MinMaxCurve(1f, curve);

        particulasJetpack.Stop();
    }

    public void SetActivo(bool activo)
    {
        if (particulasJetpack == null) return;

        if (activo && !particulasJetpack.isPlaying)
            particulasJetpack.Play();
        else if (!activo && particulasJetpack.isPlaying)
            particulasJetpack.Stop();
    }
}
