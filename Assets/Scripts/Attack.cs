using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EAttackMethod
{
    ERROR,

    Forward,
    Mortar

}

public class Attack : MonoBehaviour
{
    public GameObject attackVisual = null;

    public EAttackMethod launchMethod = EAttackMethod.Forward;

    public float cooldownMax = 0.5f;
    float cooldownCurr = 0;

    public float projectileSpeed = 10;
    public float projectileAngle = 60;

    void Update()
    {
        if (cooldownCurr > 0)
            cooldownCurr -= Time.deltaTime;
    }

    public void Fire(Vector3 targetPos)
    {
        if (GetComponent<BaseEntity>().currentHP <= 0)
            return;

        if (cooldownCurr > 0 && !DebugOverlay.RapidFireOn)
            return;

        PrepProjectile(targetPos);

        cooldownCurr = cooldownMax;
    }

    protected virtual void PrepProjectile(Vector3 targetPos)
    {
        Rigidbody newOne = ((GameObject)Instantiate(attackVisual, transform.position, Quaternion.identity)).GetComponent<Rigidbody>();

        Vector3 veloc;

        switch(launchMethod)
        {
            case EAttackMethod.Forward:
                Shoot(targetPos - transform.position, out veloc);
                newOne.useGravity = false;
                break;

            case EAttackMethod.Mortar:
                    Lob(targetPos - transform.position, out veloc);
                newOne.useGravity = true;
                break;

            default:
                veloc = Vector3.zero;
                break;
        }

        newOne.velocity = veloc;

        Projectile newProj = newOne.GetComponent<Projectile>();
        BaseEntity firingEntity = GetComponent<BaseEntity>();

        newProj.owner = firingEntity;
        newProj.gameObject.layer = LayerMask.NameToLayer((firingEntity.IsInParty ? "PartyProjectile" : "EnemyProjectile"));
    }

    void Shoot(Vector3 ToThem, out Vector3 newVelocity)
    {
        newVelocity = ToThem.normalized * projectileSpeed;
    }

    void Lob(Vector3 ToThem, out Vector3 newVelocity)
    {
        float resultingSpeed = Mathf.Sqrt((Physics.gravity.magnitude * ToThem.magnitude) / Mathf.Sin(2 * Mathf.Deg2Rad * projectileAngle));
        newVelocity = Quaternion.AngleAxis(projectileAngle, Vector3.Cross(ToThem.normalized, transform.up)) * ToThem.normalized * resultingSpeed;
    }


    void LawOfSines(Vector3 ToThem, out Vector3 FireDirection)
    {
        float resultingAngle;

        if (true)
            resultingAngle = 0;


        FireDirection = Vector3.zero * resultingAngle;
    }
}
