using UnityEngine;
using System.Collections;

public class EntityDeathBehavior : BaseDeathBehavior
{
    public GameObject deathEffect;

    public override void OnDeath()
    {
        StartCoroutine(LaunchExplodeDeath());
    }

    IEnumerator LaunchExplodeDeath()
    {
        GetComponent<NavMeshAgent>().enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = false;
        rb.AddForce((Random.insideUnitSphere + (Vector3.up*2)).normalized * 50000);
        rb.maxAngularVelocity = Mathf.Infinity;
        rb.angularVelocity = Random.insideUnitSphere * Random.Range(20, 80);

        yield return new WaitForSeconds((FindObjectOfType<BaseOverseer>().allPartyDied ? 3 : 1));

        if (deathEffect != null)
        {
            ParticleSystem[] AllInside =
                ((GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity)).GetComponentsInChildren<ParticleSystem>();

            BaseOverseer overseer = FindObjectOfType<BaseOverseer>();
            if(overseer.allPartyDied && GetComponent<BaseEntity>() == overseer.leader)
            {
                foreach (ParticleSystem curr in AllInside)
                    curr.startLifetime *= 5;
            }
        }

        Destroy(gameObject);
    }
}
