using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugOverlay : MonoBehaviour
{
    float FPSDeltaTime = 0;

    Text FPS;
    Text RapidFire;
    Text AllFire;
    Text DamageType;

    static public bool RapidFireOn = false;
    static public bool AllFireOn = false;
    ElementType DamageElement = ElementType.Neutral;

    // Use this for initialization
    void Start()
    {
        FPS = transform.GetChild(0).GetComponentInChildren<Text>();
        RapidFire = transform.GetChild(1).GetComponentInChildren<Text>();
        AllFire = transform.GetChild(2).GetComponentInChildren<Text>();
        DamageType = transform.GetChild(3).GetComponentInChildren<Text>();

        UpdateRapidFire();
        UpdateAllFire();
        UpdateDamageType();
    }

    // Update is called once per frame
    void Update()
    {
        FPSDeltaTime += (Time.deltaTime - FPSDeltaTime) * 0.1f;

        if (FindObjectOfType<BaseOverseer>().allPartyDied)
            return;

        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            RapidFireOn = !RapidFireOn;
            UpdateRapidFire();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AllFireOn = !AllFireOn;
            UpdateAllFire();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKey(KeyCode.Keypad4))
        {
            FindObjectOfType<BaseOverseer>().leader.TakeDamage(1, DamageElement);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            DamageElement = (ElementType)((((int)DamageElement) % ((int)ElementType.ElementEnd - 1)) + 1);
            UpdateDamageType();
        }
    }

    void LateUpdate()
    {
        float millis = FPSDeltaTime * 1000;
        float fps = 1 / FPSDeltaTime;
        FPS.text = " FPS: " + string.Format("{0:0.0} ms ({1:0.} fps)", millis, fps);
    }


    void UpdateRapidFire()
    {
        RapidFire.text = " Rapid Fire: " + (RapidFireOn ? "On" : "Off");
    }

    void UpdateAllFire()
    {
        AllFire.text = " All Fire At Once: " + (AllFireOn ? "On" : "Off");
    }

    void UpdateDamageType()
    {
        DamageType.text = " Debug Damage Type:\n " + DamageElement.ToString();
    }
}
