using UnityEngine;
using System.Collections;

public class PlayerControl : BaseBehavior
{
    public float moveSpeed = 7.5f;

    Transform cam;
    NavMeshAgent my;

    MouseInput mouse = null;

    // Use this for initialization
    void Awake()
    {
        cam = Camera.main.transform;
        my = GetComponent<NavMeshAgent>();
        mouse = FindObjectOfType<BaseOverseer>().GetComponentInChildren<MouseInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PersistentGameManager.IsInCutscene)
            return;

        float UD = Input.GetAxis("Vertical"), LR = Input.GetAxis("Horizontal");
        if (Mathf.Abs(UD) > 0 || Mathf.Abs(LR) > 0)
            Move(UD, LR);

        if (Input.GetButton("Strafe"))
            Strafe();

        if (Input.GetMouseButton(1))
            Attack();
    }

    void Move(float UD, float LR)
    {
        const float bigEpsilon = 0.002f;

        if (Mathf.Abs(UD) > bigEpsilon || Mathf.Abs(LR) > bigEpsilon)
        {
            Vector3 camForward = cam.forward, camRight = cam.right;
            camForward.y = camRight.y = 0;

            Vector3 resultingDir = ((camForward.normalized * UD) + (camRight.normalized * LR));

            if (resultingDir.sqrMagnitude > 1)
                resultingDir.Normalize();

            my.velocity = resultingDir * moveSpeed;
        }
    }

    void Strafe()
    {
        Vector3 newForward = (mouse.AlignedMousePos - transform.position);
        newForward.y = 0;
        transform.forward = newForward.normalized;
    }

    void Attack()
    {
        if (DebugOverlay.AllFireOn)
        {
            Attack[] allAtks = FindObjectsOfType<Attack>();

            foreach (Attack curr in allAtks)
                curr.Fire(mouse.transform.position);
        }
        else
            GetComponent<Attack>().Fire(mouse.transform.position);
    }


    public override void OnBehaviorEnable()
    {
        my.avoidancePriority = 0;
    }

    public override void OnBehaviorDisable()
    {
        my.avoidancePriority = 50;
    }
}
