using UnityEngine;
using System.Collections;

public class RemoveOnNoParticlesLeft : MonoBehaviour
{
    ParticleSystem psys;
    // Use this for initialization
    void Start()
    {
        psys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (psys.particleCount <= 0)
            Destroy(gameObject);
    }
}
