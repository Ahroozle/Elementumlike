using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthPiece : MonoBehaviour
{
    public Color drainedColor = Color.gray;

    Image my;

    // Use this for initialization
    void Awake()
    {
        my = GetComponent<Image>();
    }

    void Start()
    {
        if (GetComponentInParent<BaseEntity>().MaxHP >= 35) // transition from discrete packets to smooth bar when max hp is 35+
            GetComponent<Shadow>().effectDistance = new Vector2(0, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Drain()
    {
        my.color = drainedColor;

        //StartCoroutine(Recoil());
    }

    public void Fill(Color newcol)
    {
        my.color = newcol;
    }

    IEnumerator Recoil()
    {
        float shakeStrength = 0.5f;
        const float shakeDampening = 2.0f;

        Vector3 origLocalPos = transform.localPosition;
        
        while(shakeStrength > 0)
        {
            transform.localPosition = origLocalPos + ((Vector3)(Random.insideUnitCircle) * shakeStrength);

            shakeStrength -= shakeDampening * Time.deltaTime;

            yield return null;
        }

        transform.localPosition = origLocalPos;
    }
}
