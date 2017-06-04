using UnityEngine;
using System.Collections;

public class SelectionIndicator : MonoBehaviour
{
    [HideInInspector] public BaseEntity SelectedCreature;

    [HideInInspector] public bool ForParty = false;

    // Use this for initialization
    void Start()
    {
        Material mat = transform.GetChild(0).GetComponent<Renderer>().material;
        Color col = mat.GetColor("_Color");

        col.r = (ForParty ? 0 : 1);
        col.g = col.b = (ForParty ? 1 : 0);

        mat.SetColor("_Color", col);

        transform.GetChild(0).GetComponent<TrailRenderer>().material.SetColor("_Color", col);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = SelectedCreature.transform.position;
    }
}
