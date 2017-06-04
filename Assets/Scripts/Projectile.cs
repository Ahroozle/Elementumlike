using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public BaseEntity owner = null;
    public GameObject DieEffect = null;

    bool isMortared = false;

    public float lifetime = 5;
    public GameObject trail = null;

    void Start()
    {
        if (isMortared)
            GetComponent<Rigidbody>().useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMortared)
        {
            if (lifetime <= 0)
                Die(null);
            else
                lifetime -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        Transform highest = coll.transform;

        while (highest.parent != null)
            highest = highest.parent;

        BaseEntity hitCreature = highest.GetComponentInChildren<BaseEntity>();
        Projectile hitProj = coll.GetComponentInChildren<Projectile>();

        if ((hitCreature != null && owner != null && hitCreature.IsInParty == owner.IsInParty) || hitProj != null)
            return;

        Die(hitCreature);
    }

    void Die(BaseEntity target)
    {
        if (target != null && !PersistentGameManager.IsInCutscene)
        {
            // apply damage yadda yadda
            target.TakeDamage(1, owner.Element);
        }

        // die in a particle effect

        if (DieEffect != null)
        {
            Vector3 HitPos;

            RaycastHit inf;

            Vector3 velDir = GetComponent<Rigidbody>().velocity.normalized;

            bool hitSomething =
                Physics.SphereCast(new Ray(transform.position - velDir, velDir), GetComponent<SphereCollider>().radius * transform.localScale.x, out inf, 1, 1);

            if (hitSomething)
                HitPos = inf.point;
            else
                HitPos = transform.position;

            GameObject effect = (GameObject)Instantiate(DieEffect, HitPos, Quaternion.identity);

            if (hitSomething)
                effect.transform.up = inf.normal;
        }

        if(trail != null)
        {
            trail.transform.SetParent(null);
            trail.AddComponent<RemoveOnNoParticlesLeft>();
        }

        Destroy(gameObject);
    }
}
