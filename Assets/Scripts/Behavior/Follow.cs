using UnityEngine;
using System.Collections;

public class Follow : BaseBehavior
{
    public Transform EntityToFollow;

    NavMeshAgent my;

    // Use this for initialization
    void Awake()
    {
        my = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (my.destination != EntityToFollow.position && my.isOnNavMesh)
            my.SetDestination(EntityToFollow.position);

        Vector3 toEntity = (EntityToFollow.position - transform.position);
        if (toEntity.magnitude <= my.stoppingDistance)
        {
            toEntity.y = 0;
            transform.forward = Vector3.Slerp(transform.forward, toEntity.normalized, 8 * Time.deltaTime);
        }
    }

    public override void OnBehaviorEnable()
    {
    }

    public override void OnBehaviorDisable()
    {
        my.ResetPath();
    }
}
