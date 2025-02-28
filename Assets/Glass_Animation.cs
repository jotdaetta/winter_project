using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass_Animation : MonoBehaviour
{
    Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (false)
            Animator.SetBool("Broke", true);
    }
}
