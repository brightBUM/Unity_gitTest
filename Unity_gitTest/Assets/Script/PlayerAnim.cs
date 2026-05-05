using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    //[SerializeField] float cooldownTime = 2f;
    [SerializeField] static int noOfClicks = 0;
    float nextFireTime = 0f;
    float lastClickTime = 0f;
    float maxComboDelay = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Sd_Atk"))
        {
            animator.SetBool("Attack", false);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Spin_Atk"))
        {
            animator.SetBool("ComboAttack", false);
            noOfClicks = 0;
        }

        if(Time.time - lastClickTime > maxComboDelay)
        {
            noOfClicks = 0;
        }
        if(Time.time > nextFireTime)
        {
            if(Input.GetMouseButtonDown(0))
            {
                OnClick();
                
            }
        }
    }
    void OnClick()
    {
        lastClickTime = Time.time; 
        noOfClicks++;
        if(noOfClicks == 1 )
        {
            animator.SetBool("Attack", true);
        }
        if(noOfClicks == 2)
        {
            animator.SetBool("Attack", true);
        }
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        if(noOfClicks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Sd_Atk"))
        {
            animator.SetBool("Attack", false);
            animator.SetBool("ComboAttack", true);
        }
    }
}
