using UnityEngine;

public class NpcCorriendo : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("IsGrounded", true);
        anim.SetFloat("VelZ", 1.0f);
        anim.SetFloat("VelX", 0.0f);
        anim.SetFloat("VerticalVelocity", 0.0f);
    }
}
