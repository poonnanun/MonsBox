using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchActive : MonoBehaviour
{
    Animator animator;

    void Start(){
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void OnMouseDown() {
        animator.SetInteger("AnimState", 1);
        print("touch");
        
    }

    public void ToIdleState(){
        animator.SetInteger("AnimState", 0);
        print("stop");
    }
}
