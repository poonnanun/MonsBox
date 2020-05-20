using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchActive : MonoBehaviour
{
    Animator animator;
    private float counter = 0f;

    void Start(){
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void OnMouseDown() {
        animator.SetBool("Touch", true);
        print("touch");
        
    }

    void FixedUpdate() {
        
        if(animator.GetBool("Touch") == true){
            counter += Time.deltaTime;
            if(counter >= 0.15f){
                counter = 0f;
                animator.SetBool("Touch", false);
            }
        }
    }
}
