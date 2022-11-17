using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInputManager : MonoBehaviour, IPointerClickHandler
{
    public bool IncludeNumber = false;
    public int ArrayIndex;

    [SerializeField] string NumberValue;
    [SerializeField] float DetectionRadius = 10f;
    [SerializeField] LayerMask mask;
    [SerializeField] Transform colPos;

    TMP_Text numTxt;

    LevelManager levelManager;
    int MaxStars, CurtStars = 0;
    Image img;
    Color tempCol, CurtActiveCol;

    [HideInInspector] public bool HoldStar = false, IsValid = false;

    [SerializeField] List<ButtonInputManager> curtNeighbours;

    private void Start()
    {
        img = GetComponent<Image>();
        levelManager = FindObjectOfType<LevelManager>();
        numTxt = GetComponentInChildren<TMP_Text>();
        tempCol = img.color;
        CurtActiveCol = tempCol;
    }

    public void InsertNumber(string _value)
    {
        NumberValue = _value;
        IncludeNumber = true;
        if(numTxt!=null)
        numTxt.text = NumberValue;
        if(levelManager!=null)
        levelManager.UpdateNeighbour += UpdateNeighbourData;
        MaxStars = (int.TryParse(NumberValue, out MaxStars)) ? MaxStars : 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AddStar(!HoldStar);
    }

    public void AddStar(bool _value)
    {
        if (IncludeNumber)
            return;
        if (!HoldStar && _value)
        {
            levelManager.UsedStars--;
        }
        if (HoldStar && !_value)
        {
            levelManager.UsedStars++;
        }
        HoldStar = _value;
        numTxt.text = (HoldStar) ? "*" : "";
        levelManager.ValueUpdated();
    }
    void GetNeighbours()
    {
        var _tempButt = levelManager.mang;
        int _tempIndex = ArrayIndex + 1;

        #region Neighbour Calculation
        // bottom row
        if (_tempIndex <= 7)//bottom most
        {
            curtNeighbours.Add(_tempButt[ArrayIndex + 7]);//top
            if (_tempIndex != 7)//not right most
            {
                curtNeighbours.Add(_tempButt[ArrayIndex + 1]);//right
                curtNeighbours.Add(_tempButt[ArrayIndex + 8]);//digonals
            }
            if (_tempIndex != 1)
            {
                curtNeighbours.Add(_tempButt[ArrayIndex - 1]);//left
                curtNeighbours.Add(_tempButt[ArrayIndex + 6]);//digonals
            }
        }

        // middle rows
        else if (_tempIndex > 7 && _tempIndex < 42)
        {
            curtNeighbours.Add(_tempButt[ArrayIndex + 7]);//top
            if (_tempIndex % 7 != 0)//not right most
            {
                curtNeighbours.Add(_tempButt[ArrayIndex + 1]);//right
                curtNeighbours.Add(_tempButt[ArrayIndex + 8]);//top digonals
                curtNeighbours.Add(_tempButt[ArrayIndex - 6]);//bottom digonals
            }
            if ((_tempIndex - 1) % 7 != 0)
            {
                curtNeighbours.Add(_tempButt[ArrayIndex - 1]);//left
                curtNeighbours.Add(_tempButt[ArrayIndex + 6]);//top digonals
                curtNeighbours.Add(_tempButt[ArrayIndex - 8]);//Bottom digonals
            }
            curtNeighbours.Add(_tempButt[ArrayIndex - 7]);// bottom 
        }

        //top most
        else
        {
            if (_tempIndex % 7 != 0)//not right most
            {
                curtNeighbours.Add(_tempButt[ArrayIndex + 1]);//right
                curtNeighbours.Add(_tempButt[ArrayIndex - 6]);//bottom digonals
            }
            if ((_tempIndex - 1) % 7 != 0)
            {
                curtNeighbours.Add(_tempButt[ArrayIndex - 1]);//left
                curtNeighbours.Add(_tempButt[ArrayIndex - 8]);//Bottom digonals
            }
            curtNeighbours.Add(_tempButt[ArrayIndex - 7]);// bottom 
        }
        #endregion
    }
    void UpdateNeighbourData()
    {
        CurtStars = 0;
        if (curtNeighbours.Count == 0)
        {
            GetNeighbours();
        }
        foreach (var neighbour in curtNeighbours)
        {
            if (neighbour != null)
            {
                if (neighbour.HoldStar)
                {
                    CurtStars++;
                }
            }
        }
        IsValid = (CurtStars == MaxStars);
        if (CurtStars > MaxStars)
        {
            CurtActiveCol = Color.red;
            UpdateColor(Color.red);
        }
        else
        {
            CurtActiveCol = tempCol;
            UpdateColor();
        }
    }

    public void UpdateColor()
    {
        img.color = CurtActiveCol;
    }
    public void UpdateColor(Color _col)
    {
        if (img != null)
            img.color = _col;
    }
}
