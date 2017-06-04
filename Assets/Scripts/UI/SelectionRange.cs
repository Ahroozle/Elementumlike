using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionRange : MonoBehaviour
{
    public GameObject SelectedIndicator = null;

    [HideInInspector] public List<SelectionIndicator> AllInRange;

    [HideInInspector] public bool ForParty = false;

    bool rangeCemented = false;

    public bool IsEmpty { get { return AllInRange == null || AllInRange.Count < 1; } }

    // Use this for initialization
    void Start()
    {
        Color chosenColor = (ForParty ? PersistentGameManager.PartyColor : PersistentGameManager.EnemyColor);
        transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", chosenColor);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (rangeCemented)
            return;

        BaseEntity nextToSelect = coll.GetComponent<BaseEntity>();

        if(nextToSelect)
        {
            if ((ForParty && nextToSelect.IsInParty) || (!ForParty && !nextToSelect.IsInParty))
                Select(nextToSelect);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (rangeCemented)
            return;

        BaseEntity nextToDeselect = coll.GetComponent<BaseEntity>();

        if (nextToDeselect)
        {
            Deselect(nextToDeselect);
        }
    }

    void Select(BaseEntity newBlood)
    {
        PlayerControl theirPlayerControl = newBlood.GetBehaviorComponent<PlayerControl>();
        if(newBlood.IsInParty && (theirPlayerControl == null || !theirPlayerControl.enabled))
        {
            SelectionIndicator newOne =
                ((GameObject)Instantiate(SelectedIndicator, newBlood.transform.position, Quaternion.identity)).GetComponent<SelectionIndicator>();
            newOne.ForParty = ForParty;
            newOne.SelectedCreature = newBlood;

            AllInRange.Add(newOne);
        }
    }

    void Deselect(BaseEntity leaving)
    {
        for(int i = AllInRange.Count-1; i >= 0; --i)
        {
            if(AllInRange[i].SelectedCreature == leaving)
            {
                Destroy(AllInRange[i].gameObject);
                AllInRange.RemoveAt(i);
                return;
            }
        }
    }

    public void DeselectAllAndTerminate()
    {
        foreach (SelectionIndicator curr in AllInRange)
        {
            curr.transform.parent = null;
            curr.StartCoroutine(PersistentGameManager.FadeOutAndDestroy(curr.gameObject, 1));
        }

        AllInRange.Clear();

        StartCoroutine(PersistentGameManager.FadeOutAndDestroy(gameObject, 2));
    }
    
    public void CementRange()
    {
        //GetComponentInChildren<Renderer>().enabled = false;

        rangeCemented = true;
    }
}
