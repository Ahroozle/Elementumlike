using UnityEngine;
using System.Collections;

// This class is for death-related things like animations, VFX, etc.
public class BaseDeathBehavior : BaseBehavior
{
    public virtual void OnDeath()
    {
        Debug.LogWarning(GetType().ToString() +
            ": Behavior does not override BaseDeathBehavior::OnDeath()! This may cause unintended behavior if not remedied.");
    }

    public override void OnBehaviorEnable()
    {
    }

    public override void OnBehaviorDisable()
    {
    }
}
