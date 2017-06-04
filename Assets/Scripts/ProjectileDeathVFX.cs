using UnityEngine;
using System.Collections;

public class ProjectileDeathVFX : MonoBehaviour
{
    ParticleSystem[] allparts;


    // Use this for initialization
    void Start()
    {
        allparts = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem curr in allparts)
        {
            curr.Emit(1000);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool anyAlive = false;

        foreach (ParticleSystem curr in allparts)
            anyAlive = anyAlive || curr.particleCount > 0;

        if (!anyAlive)
            Destroy(gameObject);
    }
}
