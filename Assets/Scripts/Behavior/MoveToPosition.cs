using UnityEngine;
using System.Collections;

public class MoveToPosition : BaseBehavior
{
    NavMeshAgent my;

    [HideInInspector] float originalStoppingDistance;

    // Use this for initialization
    void Awake()
    {
        my = GetComponent<NavMeshAgent>();
        originalStoppingDistance = my.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (my.hasPath && !my.pathPending)
        {
            if (my.remainingDistance <= my.stoppingDistance && my.pathStatus == NavMeshPathStatus.PathComplete)
            {
                my.ResetPath();
                GetComponent<BaseEntity>().Enable<WaitForLeader>();
                GetComponent<BaseEntity>().Disable<MoveToPosition>();
            }
        }
    }

    public override void OnBehaviorEnable()
    {
        my.stoppingDistance = originalStoppingDistance / 10;
    }

    public override void OnBehaviorDisable()
    {
        my.stoppingDistance = originalStoppingDistance;
    }

    public void SetDestination(Vector3 newDest)
    {
        my.SetDestination(newDest);
    }
}
