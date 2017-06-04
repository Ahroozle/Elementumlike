using UnityEngine;
using System.Collections;

public class BaseBehavior : MonoBehaviour
{
    // INTENTIONALLY EMPTY CLASS TO DERIVE BEHAVIORS FROM

    public virtual void OnBehaviorEnable()
    {
        Debug.LogWarning(GetType().ToString() +
            ": Behavior does not override BaseBehavior::OnBehaviorEnable()! This may cause unintended behavior if not remedied.");
    }

    public virtual void OnBehaviorDisable()
    {
        Debug.LogWarning(GetType().ToString() +
            ": Behavior does not override BaseBehavior::OnBehaviorDisable()! This may cause unintended behavior if not remedied.");
    }
}

