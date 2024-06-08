using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public float animationSpeed;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator.speed = animationSpeed;
    }

    private void OnGUI()
    {
        animator.speed = animationSpeed;
    }
}