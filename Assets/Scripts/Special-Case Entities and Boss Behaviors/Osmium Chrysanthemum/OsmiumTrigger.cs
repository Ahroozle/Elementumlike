using UnityEngine;
using System.Collections;

public class OsmiumTrigger : MonoBehaviour
{
    public OsmiumChrysanthemum toTrigger;

    void OnTriggerEnter(Collider coll)
    {
        BaseEntity ent = coll.GetComponent<BaseEntity>();

        if (ent != null && ent.IsInParty)
        {
            toTrigger.PlayEmerge();
            Destroy(gameObject);
        }
    }
}
