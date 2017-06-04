using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

    CONTROLS

    WASD to move

    Hold Shift to strafe

    Q+E to cycle through party

    LClick to select a non-player follower
        +Drag - select multiple non-player followers
    
    LClick an enemy after selecting followers to attack it
        +Drag - select multiple enemies to attack
        (if no enemy is present, all selected will move toward the clicked point)

    RClick to cancel all special commands.

*/

public class BaseOverseer : MonoBehaviour
{

    public List<BaseEntity> party = null;
    int leaderIndex = 0;
    public BaseEntity leader;

    Transform camHolster = null;
    //Transform cam = null;
    Transform leaderIndicator = null;
    Transform worldMouse = null;
    UnityEngine.UI.Image BlackScreen = null;

    [HideInInspector] public bool allPartyDied = false;
    bool transitionToGameOver = false;

    void Awake()
    {
        camHolster = transform.GetChild(0);
        //cam = camHolster.GetChild(0);
        leaderIndicator = transform.GetChild(1);
        worldMouse = transform.GetChild(2);
        BlackScreen = transform.GetChild(5).GetComponentInChildren<UnityEngine.UI.Image>();

        if (party == null)
            party = new List<BaseEntity>();

        StartCoroutine(FadeIn());
    }

    // Use this for initialization
    void Start()
    {
        if (leader != null)
        {
            leader.Enable<PlayerControl>();

            if (!party.Contains(leader))
                party.Add(leader);

            Follow currFollow;
            foreach (BaseEntity curr in party)
            {
                if (curr != leader)
                {
                    currFollow = curr.Enable<Follow>();
                    currFollow.EntityToFollow = leader.transform;
                }

                curr.NowInParty();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PersistentGameManager.IsInCutscene)
        {
            camHolster.position = Vector3.Lerp(camHolster.position, PersistentGameManager.CutsceneFocusPoint, Time.deltaTime);
        }
        else
        {
            if (leader == null && allPartyDied)
            {
                //TODO: GAME OVER
                if (!transitionToGameOver)
                {
                    StartCoroutine(GameOver());
                    transitionToGameOver = true;
                }

                return;
            }

            UpdatePositions();

            CheckIfSwitchingLeaders();
        }
    }

    void UpdatePositions()
    {
        const float CamBetweenRatio = 0.3f;
        const float SeeDist = 40;

        float slowLerpDelta = 4 * Time.deltaTime;
        float fastLerpDelta = 16 * Time.deltaTime;

        Vector3 leaderPos = leader.transform.position;

        transform.position = Vector3.Lerp(transform.position, leaderPos, slowLerpDelta);

        leaderIndicator.position = Vector3.Lerp(leaderIndicator.position, leaderPos, fastLerpDelta);

        Vector3 worldmousealigned = worldMouse.GetComponent<MouseInput>().AlignedMousePos;//new Vector3(worldMouse.position.x, leaderPos.y, worldMouse.position.z);

        Vector3 toWMA = worldmousealigned - leaderPos;
        if (toWMA.sqrMagnitude > SeeDist * SeeDist)
            worldmousealigned = leaderPos + Vector3.ClampMagnitude(toWMA, SeeDist);


        if (!Input.GetMouseButton(0))
        {
            camHolster.position =
                Vector3.Lerp(camHolster.position, Vector3.Lerp(leaderPos, worldmousealigned, CamBetweenRatio), fastLerpDelta);
        }

    }

    void CheckIfSwitchingLeaders()
    {
        if (Input.GetButtonDown("SwitchLeaderLeft"))
            SwitchLeader(false);
        else if (Input.GetButtonDown("SwitchLeaderRight"))
            SwitchLeader(true);
    }

    void SwitchLeader(bool toRight)
    {

        int offset = (toRight ? 1 : -1);
        leaderIndex = (party.Count + (leaderIndex + offset)) % party.Count;

        BaseEntity oldLeader = leader;
        leader = party[leaderIndex];

        if (oldLeader != leader)
        {
            oldLeader.Disable<PlayerControl>();
            oldLeader.Enable<Follow>().EntityToFollow = leader.transform;

            leader.Disable<WaitForLeader>();
            leader.Disable<MoveToPosition>();
            leader.Disable<Follow>();
            leader.Enable<PlayerControl>();
            

            foreach(BaseEntity curr in party)
            {
                if (curr != oldLeader && curr != leader)
                    curr.GetBehaviorComponent<Follow>().EntityToFollow = leader.transform;
            }
        }
    }

    public void AddToParty(BaseEntity newBlood)
    {
        newBlood.DisableAllOfType<BaseEnemyBehavior>();
        newBlood.Enable<Follow>().EntityToFollow = leader.transform;
        party.Add(newBlood);
    }

    public void RemoveFromParty(BaseEntity leaving)
    {
        // This is really only intended to be called on death for now.
        // May be edited later to allow for "stealing" party members or some shit.

        if (allPartyDied)
            return;

        if (party.Count == 1)
            allPartyDied = true;

        if (leaving == leader)
        {
            int prevLeaderIndex = leaderIndex;
            SwitchLeader(true);

            if (prevLeaderIndex < leaderIndex)
                --leaderIndex;
        }

        leaving.DisableAllOfType<BaseBehavior>();

        party.Remove(leaving);
    }

    public List<string> GetPartyNames()
    {
        List<string> names = new List<string>();

        foreach (BaseEntity curr in party)
            names.Add(curr.SpeciesName);

        return names;
    }


    IEnumerator GameOver()
    {

        yield return new WaitForSeconds(3.5f);

        Animator leaderIndicatorAnimator = leaderIndicator.GetComponentInChildren<Animator>();

        leaderIndicatorAnimator.speed /= 3;

        leaderIndicatorAnimator.enabled = true;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }


    const float fadeTime = 3.0f;

    IEnumerator FadeIn()
    {
        float currTime = 0;
        float maxTime = fadeTime/2;
        while (currTime < maxTime)
        {
            currTime += Time.deltaTime;
            BlackScreen.color = Color.Lerp(Color.black, Color.clear, Mathf.Clamp01(currTime/maxTime));
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float currTime = 0;
        float maxTime = fadeTime;
        while (currTime < maxTime)
        {
            currTime += Time.deltaTime;
            BlackScreen.color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp01(currTime/maxTime));
            yield return null;
        }

        yield return new WaitForSeconds(1);
        Application.LoadLevel(Application.loadedLevel);
    }
}
