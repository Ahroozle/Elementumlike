using UnityEngine;
using System.Collections;

public class EntityHUD : MonoBehaviour
{
    Quaternion origRotation;

    Transform visualTransform;

    const float scalingSpeed = 8;

    // Use this for initialization
    void Start()
    {
        origRotation = transform.rotation;
        visualTransform = transform.GetChild(0);
        visualTransform.localScale = Vector3.zero;

        Sprite myElementImg =
            FindObjectOfType<PersistentGameManager>().ElementImages[((int)GetComponentInParent<BaseEntity>().Element) - 1];

        visualTransform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = myElementImg;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = origRotation;
    }

    public void ShowHUD()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleHUD(Vector3.one,scalingSpeed));
    }

    public void HideHUD()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleHUD(Vector3.zero, scalingSpeed));
    }

    IEnumerator ScaleHUD(Vector3 newScale, float scaleSpeed)
    {
        while(Mathf.Abs(visualTransform.localScale.sqrMagnitude - newScale.sqrMagnitude) > 0.002f)
        {
            visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, newScale, scaleSpeed * Time.deltaTime);

            yield return null;
        }

        visualTransform.localScale = newScale;
    }
}
