using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInputManager : MonoBehaviour, IPointerClickHandler
{
    public bool IncludeNumber = false;
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

    [SerializeField] ButtonInputManager[] curtNeighbours;

    private void Start()
    {
        img = GetComponent<Image>();
        levelManager = FindObjectOfType<LevelManager>();
        numTxt = GetComponentInChildren<TMP_Text>();
        tempCol = img.color;
        CurtActiveCol = tempCol;
        if (IncludeNumber)
        {
            numTxt.text = NumberValue;
            levelManager.UpdateNeighbour += UpdateNeighbourData;
            MaxStars = (int.TryParse(NumberValue, out MaxStars)) ? MaxStars : 0;
        }
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
        Collider[] _tempChilds = Physics.OverlapSphere(transform.position, DetectionRadius);
        curtNeighbours = new ButtonInputManager[_tempChilds.Length];
        for (int i = 0; i < curtNeighbours.Length; i++)
        {
            if (_tempChilds[i].transform.parent.GetComponent<ButtonInputManager>() != this)
                curtNeighbours[i] = _tempChilds[i].transform.parent.GetComponent<ButtonInputManager>();
        }
    }
    void UpdateNeighbourData()
    {
        CurtStars = 0;
        GetNeighbours();
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
