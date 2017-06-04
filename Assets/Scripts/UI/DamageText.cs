using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public int damageAmount;
    public ElementType elementType = ElementType.Neutral;
    Text signage;
    Image elementImage;
    Transform HPTotal;

    float riseSpeed = 2;
    float lifeTime = 5;
    float currLife = 0;

    // Use this for initialization
    void Start()
    {
        const float Xoffset = 1.25f;
        signage = transform.GetChild(0).GetComponent<Text>();
        elementImage = transform.GetChild(1).GetComponent<Image>();
        HPTotal = transform.GetChild(2);

        RectTransform currTrans;
        for(int i = 0; i < damageAmount; ++i)
        {
            currTrans = ((GameObject)Instantiate(elementImage.gameObject, HPTotal.position, HPTotal.rotation)).GetComponent<RectTransform>();

            currTrans.SetParent(HPTotal);

            currTrans.localPosition += Vector3.right * Xoffset * i;
            currTrans.localScale = Vector3.one;

            currTrans.GetComponent<Image>().color = signage.color;
        }

        elementImage.sprite = FindObjectOfType<PersistentGameManager>().ElementImages[(int)elementType - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (currLife >= lifeTime)
            Destroy(gameObject);
        else
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            currLife += Time.deltaTime;
        }
    }
}
