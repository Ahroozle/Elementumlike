using UnityEngine;
using System.Collections;

public class OsmiumHead : MonoBehaviour
{
    public GameObject EyeLaser;

    OsmiumChrysanthemum my;
    BaseOverseer player;
    Transform target;
    LineRenderer lineOfSight;
    Transform eyeBase;
    Transform laserEye;

    readonly float maxWaitTime = 5.0f;
    readonly float maxWaitVariance = 2.0f;
    float currWaitTime = 0;

    float healthRatio;

    bool firingEyeLaser = false;
    readonly float maxLaserEyeCooldown = 5.0f;
    float laserEyeCooldown = 0;

    // Use this for initialization
    void Start()
    {
        my = GetComponentInParent<OsmiumChrysanthemum>();
        player = FindObjectOfType<BaseOverseer>();
        target = player.leader.transform;
        lineOfSight = GetComponentInChildren<LineRenderer>();
        currWaitTime = maxWaitTime;
        eyeBase = transform.GetChild(0);
        laserEye = transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        healthRatio = (float)(my.currentHP) / my.MaxHP;

        currWaitTime -= Time.deltaTime * Mathf.Lerp(64, 1, healthRatio);

        if (currWaitTime <= 0 && Random.value <= 0.1f)
        {
            SwitchLookTarget();
            currWaitTime = maxWaitTime - (Random.value * maxWaitVariance);
        }
        else if (!firingEyeLaser && currWaitTime <= 1.0f && Random.value <= 0.2f)
        {
            StopAllCoroutines();
            StartCoroutine(RotateEyes((Random.value * 720.0f) - 360.0f));
        }

        if (!firingEyeLaser && laserEyeCooldown <= 0 && Random.value <= 0.5f)
        {
            StartCoroutine(FireEyeLaser());
        }
        else
        {
            laserEyeCooldown -= Time.deltaTime;

            float sightWidth = Mathf.Lerp(1.0f, 0.1f, laserEyeCooldown / maxLaserEyeCooldown);
            lineOfSight.SetWidth(sightWidth, sightWidth);
        }

        if (target == null)
            SwitchLookTarget();
        else
        {
            lineOfSight.SetPosition(1, laserEye.InverseTransformPoint(target.position));

            transform.up =
                Vector3.Slerp(transform.up, (transform.position - target.position).normalized, Mathf.Lerp(16, 1, healthRatio) * Time.deltaTime);
        }
    }

    void SwitchLookTarget()
    {
        if (player.party.Count < 1) return;

        target = player.party[Random.Range(0, player.party.Count)].transform;
    }

    IEnumerator RotateEyes(float numDegsToRotate)
    {
        if (Mathf.Abs(numDegsToRotate) > 0)
        {
            float currChomp;
            float degSign = numDegsToRotate / Mathf.Abs(numDegsToRotate);
            while (Mathf.Abs(numDegsToRotate) > 0)
            {
                currChomp = degSign * Mathf.Lerp(256, 1, healthRatio) * Time.deltaTime;

                if (Mathf.Abs(numDegsToRotate) - Mathf.Abs(currChomp) < 0)
                    currChomp = numDegsToRotate;

                eyeBase.Rotate(0, currChomp, 0, Space.Self);

                numDegsToRotate -= currChomp;

                yield return null;
            }
        }
        yield return null;
    }

    IEnumerator FireEyeLaser()
    {
        if (target != null)
        {
            firingEyeLaser = true;

            int shotsToFire = (int)(Random.Range(20, 80) * Mathf.Lerp(4, 1, healthRatio));
            Rigidbody currentOne;

            Vector3 predictedTargetPos;
            while (shotsToFire > 0)
            {
                if (target == null)
                    break;

                currentOne = ((GameObject)Instantiate(EyeLaser, laserEye.transform.position, Quaternion.identity)).GetComponent<Rigidbody>();

                predictedTargetPos = target.position + target.GetComponent<NavMeshAgent>().velocity;
                currentOne.velocity = Vector3.Lerp((predictedTargetPos - laserEye.position).normalized, -laserEye.up, healthRatio) * 10;
                currentOne.useGravity = false;
                currentOne.GetComponent<Projectile>().owner = my;

                --shotsToFire;

                yield return null;
            }

            firingEyeLaser = false;
            laserEyeCooldown = maxLaserEyeCooldown;
        }
        yield return null;
    }

    public void SetNowDead()
    {
        StopAllCoroutines();
        Destroy(lineOfSight);
        enabled = false;
    }
}
