using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Notification : MonoBehaviour
{
    public float FadeSpeed = 1.0f;

    Animator my;

    // Use this for initialization
    void Start()
    {
        my = GetComponentInChildren<Animator>();
        my.enabled = true;
    }

    void Update()
    {
        if (my.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            Destroy(gameObject);
    }

}
