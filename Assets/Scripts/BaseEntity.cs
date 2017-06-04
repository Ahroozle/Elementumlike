using UnityEngine;
using System.Collections;

public enum ElementType
{
    ERROR = 0,

    Neutral = 1,
    Water,
    Fire,
    Earth,
    Electricity,
    Magic,
    Heal,

    ElementEnd

}

public class BaseEntity : MonoBehaviour
{
    bool InParty = false;
    public bool IsInParty { get { return InParty; } }

    public string SpeciesName;
    public ElementType Element = ElementType.Neutral;

    public int currentHP = 0;
    public int MaxHP = 5;

    public GameObject SawSomethingIndicator;

    public float startShakeStrength = 0.5f;
    public float shakeDampening = 2.0f;
    float shakeStrength = 0;

    bool hasDeathThrown = false;
    public float spareableChance = 0.25f;

    // Use this for initialization
    protected virtual void Start()
    {
        currentHP = MaxHP;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    void OnMouseEnter()
    {
        if (enabled && !PersistentGameManager.IsInCutscene)
            GetComponentInChildren<EntityHUD>().ShowHUD();
    }

    void OnMouseExit()
    {
        if (enabled)
            GetComponentInChildren<EntityHUD>().HideHUD();
    }

    public T Enable<T>() where T : BaseBehavior
    {
        T mine = GetComponent<T>();

        if (mine == null)
            mine = gameObject.AddComponent<T>();
        else
            mine.enabled = true;

        mine.OnBehaviorEnable();

        return mine;
    }

    // CAUTION: MAY RETURN NULL
    public T Disable<T>() where T : BaseBehavior
    {
        T mine = GetComponent<T>();

        if (mine != null)
        {
            mine.OnBehaviorDisable();
            mine.enabled = false;
        }

        return mine;
    }

    public T GetBehaviorComponent<T>() where T : BaseBehavior
    {
        return GetComponent<T>();
    }

    public T[] EnableAllOfType<T>() where T : BaseBehavior
    {
        T[] AllOfType = GetComponents<T>();

        foreach(T curr in AllOfType)
        {
            curr.OnBehaviorEnable();
            curr.enabled = true;
        }

        return AllOfType;
    }

    public T[] DisableAllOfType<T>() where T : BaseBehavior
    {
        T[] AllOfType = GetComponents<T>();

        foreach (T curr in AllOfType)
        {
            curr.OnBehaviorDisable();
            curr.enabled = false;
        }

        return AllOfType;
    }


    public void NowInParty()
    {
        InParty = true;

        Renderer PartyIndicator = transform.GetChild(1).GetComponent<Renderer>();

        PartyIndicator.material.SetColor("_Color", PersistentGameManager.PartyColor);
    }


    public void TakeDamage(int damageAmount, ElementType hitElement)
    {
        PersistentGameManager pgMngr = FindObjectOfType<PersistentGameManager>();

        // Modify damageAmount by resistance (via persistentgamemanager?)
        damageAmount = PersistentGameManager.CalculateDamage(Element, hitElement, damageAmount);

        EntityHUD myHud = GetComponentInChildren<EntityHUD>();

        if (damageAmount > 0)
        {
            if (currentHP <= 0)
                return;

            GetComponentInChildren<HPBar>().UpdateBarForDamage(Mathf.Clamp(damageAmount,0,currentHP));

            if (shakeStrength <= 0)
                StartCoroutine(ShowGotDamaged());
            else
                shakeStrength = startShakeStrength;

            // Display damaged text
            DamageText dmgText =
                ((GameObject)Instantiate(pgMngr.DamagedText,
                                         myHud.transform.GetChild(0).position + Vector3.up,
                                         pgMngr.DamagedText.transform.rotation)).GetComponent<DamageText>();

            dmgText.elementType = hitElement;
            dmgText.damageAmount = damageAmount;
        }
        else if (damageAmount < 0)
        {
            if (currentHP < MaxHP)
            {

                GetComponentInChildren<HPBar>().UpdateBarForHeal(Mathf.Clamp(-damageAmount, 0, MaxHP - currentHP));

                // Display healed text
                DamageText healText =
                    ((GameObject)Instantiate(pgMngr.HealedText,
                                             myHud.transform.GetChild(0).position + Vector3.up,
                                             pgMngr.HealedText.transform.rotation)).GetComponent<DamageText>();

                healText.elementType = hitElement;
                healText.damageAmount = -damageAmount;
            }
        }
        else
        {
            // Do Blocked

            // Display blocked text
            DamageText blockText =
                ((GameObject)Instantiate(pgMngr.BlockedText,
                                         myHud.transform.GetChild(0).position + Vector3.up,
                                         pgMngr.BlockedText.transform.rotation)).GetComponent<DamageText>();

            blockText.elementType = hitElement;

            return;
        }

        currentHP = Mathf.Clamp(currentHP - damageAmount, 0, MaxHP);

        if (currentHP <= 0)
        {
            if (!hasDeathThrown && !InParty && Random.value < spareableChance)
            {
                ++currentHP;

                // DO THE DERP
                StartCoroutine(BecomeSpareable());

                hasDeathThrown = true;
            }
            else
                Die();
        }
    }

    IEnumerator BecomeSpareable()
    {
        DisableAllOfType<BaseBehavior>();
        transform.GetChild(1).GetComponent<Renderer>().enabled = false;

        GetComponent<NavMeshAgent>().enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = false;
        rb.angularDrag *= 10;
        rb.AddForce((Random.insideUnitSphere + (Vector3.up * 2)).normalized * 5000);
        rb.maxAngularVelocity = Mathf.Infinity;
        rb.angularVelocity = Random.insideUnitSphere * Random.Range(20, 80);

        const int shakesMax = 20;
        int shakesLeft = shakesMax;
        while(shakesLeft > 0)
        {
            if (Random.value < 0.25f * ((1 - ((float)(shakesLeft) / shakesMax)) + 0.01f))
            {
                --shakesLeft;
                StartCoroutine(DyingShake());
                yield return new WaitForSeconds(0.5f);
            }
            else
                yield return null;
        }

        Die();
    }

    IEnumerator DyingShake()
    {
        float currTime = 0;
        const float maxTime = 0.1f;
        while(currTime < maxTime)
        {
            currTime += Time.deltaTime;

            transform.Rotate(0, Mathf.Sin(Time.time * 16), 0, Space.Self);

            yield return null;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        BaseEntity collidedWith = coll.gameObject.GetComponent<BaseEntity>();
        BaseOverseer overseer = FindObjectOfType<BaseOverseer>();
        if (collidedWith == overseer.leader && hasDeathThrown)
        {
            StopAllCoroutines();

            // REVIVE
            StartCoroutine(GivenSecondChance());
        }
    }

    IEnumerator GivenSecondChance()
    {
        BaseOverseer overseer = FindObjectOfType<BaseOverseer>();
        GetComponent<NavMeshAgent>().enabled = true;
        overseer.AddToParty(this);
        NowInParty();

        TakeDamage(1000, ElementType.Heal);
        transform.GetChild(1).GetComponent<Renderer>().enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.angularDrag /= 10;

        hasDeathThrown = false;

        while(transform.up != Vector3.up)
        {
            transform.up = Vector3.Slerp(transform.up, Vector3.up, 4 * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    protected virtual void Die()
    {
        if (InParty)
            FindObjectOfType<BaseOverseer>().RemoveFromParty(this);

        DisableAllOfType<BaseBehavior>();

        Destroy(transform.GetChild(1).gameObject);

        StopAllCoroutines();

        GetBehaviorComponent<BaseDeathBehavior>().OnDeath();
    }

    IEnumerator ShowGotDamaged()
    {
        Transform visual = transform.GetChild(0);

        Vector3 origLocalPos = visual.localPosition;

        shakeStrength = startShakeStrength;
        while (shakeStrength > 0)
        {
            visual.localPosition = origLocalPos + (Vector3.ProjectOnPlane(Random.insideUnitSphere, transform.up) * shakeStrength);

            shakeStrength -= shakeDampening * Time.deltaTime;

            yield return null;
        }

        visual.localPosition = origLocalPos;
    }
}
