using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour
{
    float midRatio = 0.6f; //~2/3
    float lowRatio = 0.3f; //~1/3

    public GameObject healthPiece;

    public float pieceScaleMultiplier = 1;
    public float offsetMultiplier = 1;

    BaseEntity my;

    HealthPiece[] healthPieces;

    // Use this for initialization
    void Awake()
    {
        my = GetComponentInParent<BaseEntity>();

        healthPieces = new HealthPiece[my.MaxHP];

        //Quaternion startRot = transform.rotation;
        //Quaternion rotator = Quaternion.AngleAxis(360.0f/my.MaxHP, transform.forward);
        
        float startRot = 0;
        float rotation = 360.0f / Mathf.Clamp(my.MaxHP,0,500);
        Transform currTrans;

        Color fillCol = FindObjectOfType<PersistentGameManager>().HighHealthColor;
        for (int i = 0; i < healthPieces.Length; ++i)
        {
            currTrans = 
                (healthPieces[i] =
                    ((GameObject)Instantiate(healthPiece, transform.position, transform.rotation)).GetComponent<HealthPiece>()).transform;

            currTrans.SetParent(transform);
            currTrans.Rotate(0, 0, startRot);

            currTrans.localScale *= pieceScaleMultiplier;
            currTrans.position += currTrans.transform.up * (offsetMultiplier + (i*0.0005f));
            currTrans.Rotate(0, -30.0f, 0, Space.Self);

            startRot += rotation;

            healthPieces[i].Fill(fillCol);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateBarForDamage(int totalDamage)
    {
        PersistentGameManager PGMngr = FindObjectOfType<PersistentGameManager>();
        float HPRatio = ((float)my.currentHP-totalDamage) / my.MaxHP;
        Color nextColor = (HPRatio > midRatio ? PGMngr.HighHealthColor : (HPRatio > lowRatio ? PGMngr.MidHealthColor : PGMngr.LowHealthColor));

        for (int i = my.currentHP - 1; i >= 0; --i)
        {
            if (totalDamage > 0)
            {
                healthPieces[i].Drain();
                --totalDamage;
            }
            else
                healthPieces[i].Fill(nextColor);
        }
    }

    public void UpdateBarForHeal(int totalHealed)
    {
        PersistentGameManager PGMngr = FindObjectOfType<PersistentGameManager>();
        float HPRatio = ((float)my.currentHP + totalHealed) / my.MaxHP;
        Color nextColor = (HPRatio > midRatio ? PGMngr.HighHealthColor : (HPRatio > lowRatio ? PGMngr.MidHealthColor : PGMngr.LowHealthColor));

        for (int i = my.currentHP + totalHealed - 1; i >= 0; --i)
        {
            healthPieces[i].Fill(nextColor);
        }
    }
}
