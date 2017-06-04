using UnityEngine;
using System.Collections;

public class MouseInput : MonoBehaviour
{
    public GameObject RangeIndicator = null;

    //BaseOverseer overseer = null;

    RaycastHit inf;

    Vector3 CurrRangePosPlayerAligned;
    Vector3 MousePosPlayerAligned;
    public Vector3 AlignedMousePos { get { return MousePosPlayerAligned; } }

    SelectionRange currentRange = null;
    SelectionRange PartySelected = null;
    SelectionRange EnemiesSelected = null;

    Transform visual;

    void Awake()
    {
        visual = transform.GetChild(0);

        visual.GetComponent<Renderer>().material.SetColor("_Color", PersistentGameManager.PartyColor);
    }

    // Use this for initialization
    void Start()
    {
        //overseer = transform.parent.GetComponent<BaseOverseer>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();

        HandleClick();

    }

    void FollowMouse()
    {
        const int worldMouseLayerMask = 1 | (1 << 8); // Default | WorldMousePlane
        const int cameraMouseLayerMask = (1 << 9); // CameraMousePlane
        Ray fromMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(fromMouse, out inf, Mathf.Infinity, worldMouseLayerMask))
        {
            transform.position = inf.point;

            if (Physics.Raycast(fromMouse, out inf, Mathf.Infinity, cameraMouseLayerMask))
            {
                MousePosPlayerAligned = inf.point;
            }
        }
    }

    void HandleClick()
    {
        int caseNum = -1;

        if (Input.GetMouseButtonDown(0))
            caseNum = 0;
        else if (Input.GetMouseButton(0))
            caseNum = 1;
        else if (Input.GetMouseButtonUp(0))
            caseNum = 2;
        else
            return;

        if (PartySelected == null)
            DoParty(caseNum);
        else
            DoEnemies(caseNum);

        if (PartySelected && EnemiesSelected)
        {
            if (EnemiesSelected.IsEmpty)
                MoveSelectedToMousePosition();
            else
                DistributeTargets();
            ResetRanges();
        }

    }



    void DoParty(int caseNum)
    {
        switch (caseNum)
        {
            case 0:

                {
                    if (PartySelected == null)
                    {
                        currentRange = ((GameObject)Instantiate(RangeIndicator, transform.position, Quaternion.identity)).GetComponent<SelectionRange>();
                        currentRange.ForParty = true;
                        CurrRangePosPlayerAligned = MousePosPlayerAligned;
                    }
                }

                break;

            case 1:

                {
                    float newRange = (CurrRangePosPlayerAligned - MousePosPlayerAligned).magnitude;
                    currentRange.transform.localScale = new Vector3(newRange, 1, newRange);
                }

                break;

            case 2:

                {
                    PartySelected = currentRange;
                    currentRange = null;
                    PartySelected.CementRange();

                    visual.GetComponent<Renderer>().material.SetColor("_Color", PersistentGameManager.EnemyColor);
                }

                break;

            default:
                break;
        }
    }

    void DoEnemies(int caseNum)
    {
        switch (caseNum)
        {
            case 0:

                {
                    if (EnemiesSelected == null)
                    {
                        currentRange = ((GameObject)Instantiate(RangeIndicator, transform.position, Quaternion.identity)).GetComponent<SelectionRange>();
                        currentRange.ForParty = false;
                        CurrRangePosPlayerAligned = MousePosPlayerAligned;
                    }
                }

                break;

            case 1:

                {
                    float newRange = (CurrRangePosPlayerAligned - MousePosPlayerAligned).magnitude;
                    currentRange.transform.localScale = new Vector3(newRange, 1, newRange);
                }

                break;

            case 2:

                {
                    EnemiesSelected = currentRange;
                    currentRange = null;
                    EnemiesSelected.CementRange();

                    visual.GetComponent<Renderer>().material.SetColor("_Color", PersistentGameManager.PartyColor);
                }

                break;

            default:
                break;
        }
    }

    void MoveSelectedToMousePosition()
    {
        foreach (SelectionIndicator curr in PartySelected.AllInRange)
        {
            curr.SelectedCreature.Disable<Follow>();
            curr.SelectedCreature.Disable<WaitForLeader>();
            curr.SelectedCreature.Enable<MoveToPosition>().SetDestination(transform.position);
        }
    }

    void DistributeTargets()
    {
        if (PartySelected.AllInRange.Count < 1 || EnemiesSelected.AllInRange.Count < 1)
            return;

        //DISTRIBUTE TARGETS BASED ON:
        //  -effectiveness of party elements vs enemy elements
        //  -uniformity of distribution
    }

    void ResetRanges()
    {
        PartySelected.DeselectAllAndTerminate();
        EnemiesSelected.DeselectAllAndTerminate();

        PartySelected = null;
        EnemiesSelected = null;
    }
}
