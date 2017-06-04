using UnityEngine;
using System.Collections;

public class OsmiumChrysanthemum : BaseEntity
{
    Transform head;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        head = transform.GetChild(0);
        enabled = false;
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public void PlayEmerge()
    {
        StartCoroutine(PlayEmergeAnimation());
    }

    IEnumerator PlayEmergeAnimation()
    {
        PersistentGameManager.EnterCutscene(head.position - (head.up * 20f));

        yield return new WaitForSeconds(4.0f);

        Animator headAnimator = head.GetComponent<Animator>();

        headAnimator.speed /= 3;
        headAnimator.enabled = true;

        while (headAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        Destroy(headAnimator);

        //yield return new WaitForSeconds(2.0f);

        enabled = true;
        GetComponentInChildren<OsmiumHead>().enabled = true;

        PersistentGameManager.ExitCutscene();
    }

    protected override void Die()
    {
        head.GetComponent<OsmiumHead>().SetNowDead();

        //GetBehaviorComponent<BaseDeathBehavior>().OnDeath();
    }
}
