using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentGameManager : MonoBehaviour
{
    public GameObject DamagedText;
    public GameObject HealedText;
    public GameObject BlockedText;
    public Sprite[] ElementImages;

    public GameObject[] CreatureLookUpTable;
    Dictionary<string, GameObject> CreatureCompendium = null;

    List<string> PartyNames;

    public readonly static Color PartyColor = new Color(0, 1, 1, 0.5f);
    public readonly static Color EnemyColor = new Color(1, 0, 0, 0.5f);

    static bool cutscene = false;
    static Vector3 cutsceneFocus;
    public static bool IsInCutscene { get { return cutscene; } }
    public static Vector3 CutsceneFocusPoint { get { return cutsceneFocus; } }

    public Color HighHealthColor;
    public Color MidHealthColor;
    public Color LowHealthColor;

    void Awake()
    {
        DontDestroyOnLoad(this);

        CreatureCompendium = new Dictionary<string, GameObject>();
        foreach (GameObject currCreature in CreatureLookUpTable)
            CreatureCompendium.Add(currCreature.GetComponent<BaseEntity>().SpeciesName, currCreature);
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadStage(string lvlName)
    {
        PartyNames = GameObject.FindObjectOfType<BaseOverseer>().GetPartyNames();

        Application.LoadLevel(lvlName);
    }

    void OnLevelWasLoaded(int level)
    {
        //StartCoroutine(SpillParty());
    }

    IEnumerator SpillParty()
    {
        foreach(string currCreature in PartyNames)
        {
            //TODO: CREATE ROUGHLY NEAR LEADER, ABOVE AND OFFSCREEN.

            currCreature.Clone();

            yield return new WaitForSeconds(0.1f);
        }
    }


    #region Utility

    static public int CalculateDamage(ElementType defender, ElementType attacker, int damageAmount)
    {
        //Neutral = 1,
        //Water,
        //Fire,
        //Earth,
        //Electricity,
        //Magic,
        //Heal

        // Neutral
        //      x1 to Everything
        //      x1 from Everything
        //
        // Water
        //      x4 to Fire
        //      x2 to Earth
        //      x1 to Water
        //      x0 to Electricity
        //
        // Fire
        //      x4 to Earth
        //      x2 to Electricity
        //      x1 to Fire
        //      x0 to Water
        //
        // Earth
        //      x4 to Electricity
        //      x2 to Water
        //      x1 to Earth
        //      x0 to Fire
        //
        // Electricity
        //      x4 to Water
        //      x2 to Fire
        //      x1 to Electricity
        //      x0 to Earth
        //
        // Magic
        //      x2 to Everything (Non-Neutral, Non-Heal)
        //      x2 from Everything (Non-Neutral, Non-Heal)
        //
        // Heal
        //      x-1 to Everything
        //      x1 from Everything

        int[,] multipliers =
        {
            /*  @@@@@@@@@@@@  |  NEUTRAL  |  WATER  |  FIRE  |  EARTH  |  ELECTRICITY  |  MAGIC  |  HEAL  */ // ATTACKER
            /*  NEUTRAL      */{ 1,          1,        1,       1,        1,              1,        -1 },
            /*  WATER        */{ 1,          1,        0,       2,        4,              2,        -1 },
            /*  FIRE         */{ 1,          4,        1,       0,        2,              2,        -1 },
            /*  EARTH        */{ 1,          2,        4,       1,        0,              2,        -1 },
            /*  ELECTRICITY  */{ 1,          0,        2,       4,        1,              2,        -1 },
            /*  MAGIC        */{ 1,          2,        2,       2,        2,              2,        -1 },
            /*  HEAL         */{ 1,          1,        1,       1,        1,              1,        -1 },
            // DEFENDER
        };

        return damageAmount * multipliers[(int)defender - 1, (int)attacker - 1];
    }

    static public IEnumerator FadeIn(GameObject toFade, float fadespeed)
    {
        Material objmat = toFade.GetComponentInChildren<Renderer>().material;
        Color objcol = objmat.GetColor("_Color");
        while (objcol.a < 0.9f)
        {
            objcol.a = Mathf.Lerp(objcol.a, 0, fadespeed * Time.deltaTime);
            objmat.SetColor("_Color", objcol);
            yield return null;
        }
    }

    static public IEnumerator FadeOutAndDestroy(GameObject toDestroy, float fadespeed)
    {
        Renderer[] allRends = toDestroy.GetComponentsInChildren<Renderer>();

        Material currMat;
        Color currCol;
        bool allBelow = false;
        while (!allBelow)
        {
            foreach(Renderer currentRenderer in allRends)
            {
                currCol = (currMat = currentRenderer.material).GetColor("_Color");
                currCol.a = Mathf.Lerp(currCol.a, 0, fadespeed * Time.deltaTime);
                currMat.SetColor("_Color", currCol);

                allBelow = allBelow || currCol.a <= 0.01f;
            }

            yield return null;
        }

        Destroy(toDestroy);
    }


    public static void EnterCutscene(Vector3 focusPoint)
    {
        cutsceneFocus = focusPoint;
        cutscene = true;
    }

    public static void ExitCutscene()
    {
        cutscene = false;
    }

    #endregion
}
