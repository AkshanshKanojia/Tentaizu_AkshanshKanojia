using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform CellButtonHolder;
    [SerializeField] GameObject ClearUI;
    [SerializeField] TMP_Text StarTxt;
    bool GeneratedLevel = false;

    public delegate void NeighbourUpdated();
    public event NeighbourUpdated UpdateNeighbour;
    public int UsedStars = 10;

    public ButtonInputManager[] mang;
    int CurtActiveButton = 0;
    List<ButtonInputManager> NumberHolders = new List<ButtonInputManager>();

    [System.Serializable]
    struct LevelDetailHolder
    {
        public string LevelName;
        [Tooltip("pass index assuming not starting from 0 and value as number in that cell")]
        public List<SingleLevelDetail> LevelDetails;
    }

    [SerializeField] List<LevelDetailHolder> AvailableLevel;

    [System.Serializable]
    struct SingleLevelDetail
    {
        public int NumberIndex;
        public string NumberValue;
    }

    private void Start()
    {
        GetButtons();
        if (mang[CurtActiveButton] != null)
            mang[CurtActiveButton].UpdateColor(Color.white);
    }

    private void Update()
    {
        SetActiveButton();
        if (!GeneratedLevel)
        {
            GenerateRandomLevel();
            GeneratedLevel = true;
        }
    }
    void GenerateRandomLevel()
    {
        int _levelIndex = Random.Range(0, AvailableLevel.Count);
        foreach (var num in AvailableLevel[_levelIndex].LevelDetails)
        {
            mang[num.NumberIndex - 1].InsertNumber(num.NumberValue);
        }
    }

    void GetButtons()
    {
        mang = new ButtonInputManager[CellButtonHolder.childCount];
        for (int i = 0; i < mang.Length; i++)
        {
            mang[i] = CellButtonHolder.GetChild(i).GetComponent<ButtonInputManager>();
            mang[i].ArrayIndex = i;
            if (mang[i].IncludeNumber)
            {
                NumberHolders.Add(mang[i]);
            }
        }
    }
    void SetActiveButton()
    {
        mang[CurtActiveButton].UpdateColor();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (CurtActiveButton - 1 >= 0)
            {
                CurtActiveButton--;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CurtActiveButton + 1 < 49)
            {
                CurtActiveButton++;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (CurtActiveButton - 7 >= 0)
            {
                CurtActiveButton -= 7;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (CurtActiveButton + 7 < 49)
            {
                CurtActiveButton += 7;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            mang[CurtActiveButton].AddStar(true);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            mang[CurtActiveButton].AddStar(false);
        }
        mang[CurtActiveButton].UpdateColor(Color.white);
    }
    public void ValueUpdated()
    {
        UpdateNeighbour.Invoke();

        bool _cleared = true;
        foreach (var v in NumberHolders)
        {
            if (!v.IsValid)
            {
                _cleared = false;
                break;
            }
        }
        StarTxt.text = "Stars: " + UsedStars;
        if (UsedStars != 0)
        {
            _cleared = false;
        }
        if (_cleared)
        {
            ClearUI.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
