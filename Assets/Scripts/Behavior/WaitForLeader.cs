using UnityEngine;
using System.Collections;

public class WaitForLeader : BaseBehavior
{
    NavMeshAgent my;

    GameObject SawSomethingIndicator;

    float originalRadius;

    bool HasWaited = false;

    // Use this for initialization
    void Awake()
    {
        my = GetComponent<NavMeshAgent>();
        originalRadius = my.radius;
        SawSomethingIndicator = GetComponent<BaseEntity>().SawSomethingIndicator;
    }

    // Update is called once per frame
    void Update()
    {
        if (HasWaited)
            return;

        Transform leader = FindObjectOfType<BaseOverseer>().leader.transform;

        const float dotFOV = 0.5f; // 90-degree angle in front
        const float FOVDepthModif = 2;

        Vector3 ToLeader = (leader.position - transform.position);

        float DotRes = Vector3.Dot(transform.forward, ToLeader.normalized);
        float SqDistToLeader = ToLeader.sqrMagnitude;
        float TouchingDist = my.radius + leader.GetComponent<NavMeshAgent>().radius;
        float FOVDepth = (my.stoppingDistance * my.stoppingDistance) * FOVDepthModif;

        if (SqDistToLeader <= TouchingDist * TouchingDist || // If I get poked by the leader
            (DotRes >= dotFOV && SqDistToLeader <= FOVDepth)) // Or, if I see the leader in front of me
        {
            Instantiate(SawSomethingIndicator, transform.position, SawSomethingIndicator.transform.rotation);
            StartCoroutine(ShiftRadiusBackToNormal(0.5f));
            GetComponent<BaseEntity>().Enable<Follow>();
            HasWaited = true;
        }
    }

    public override void OnBehaviorEnable()
    {
        my.ResetPath();
        HasWaited = false;
        my.radius = 0.5f;
    }

    public override void OnBehaviorDisable()
    {
        HasWaited = true;
    }

    IEnumerator ShiftRadiusBackToNormal(float AmountOfTime)
    {
        float startingRadius = my.radius;
        float originalAmountOfTime = AmountOfTime;
        while (AmountOfTime > 0)
        {
            AmountOfTime -= Time.deltaTime;

            if (AmountOfTime < 0)
                AmountOfTime = 0;

            my.radius = Mathf.Lerp(startingRadius, originalRadius, 1 - (AmountOfTime / originalAmountOfTime));

            yield return null;
        }

        GetComponent<BaseEntity>().Disable<WaitForLeader>();
    }
}
