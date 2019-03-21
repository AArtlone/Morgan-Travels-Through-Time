using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    //booleans responsible for changing which page of the diary is shown
    //names will be changed later
    private bool _page1Selected = false;
    private bool _page2Selected = false;
    private bool _page3Selected = false;

    //references to the buttons and pages
    public GameObject _button1;
    public GameObject _button2;
    public GameObject _button3;
    public GameObject _page1;
    public GameObject _page2;
    public GameObject _page3;

    //references to the buttons orignal positions
    private Vector3 originalPosition1;
    private Vector3 originalPosition2;
    private Vector3 originalPosition3;

    //offset by which the button goes up when the correlated page is selected
    private Vector3 _offset = new Vector3(0f, 20f, 0f);
    
    // Start is called before the first frame update
    void Start()
    {
        //originalPosition1 = _button1.transform.position;
        //originalPosition2 = _button2.transform.position;
        //originalPosition3 = _button3.transform.position;
        _page1Selected = true;
        //_button1.transform.position += _offset;
        _page1.SetActive(true);
    }

    public void SelectPage(Object button)
    {
        GameObject buttonObj = button as GameObject;
        if(buttonObj.name == "Quest Button")
        {
            if(_page1Selected == false)
            {
                //_button1.transform.position += _offset;
                //_button2.transform.position = originalPosition2;
                //_button3.transform.position = originalPosition3;
                _page1Selected = true;
                _page2Selected = false;
                _page3Selected = false;
                _page1.SetActive(true);
                _page2.SetActive(false);
                _page3.SetActive(false);
            }
        } else if(buttonObj.name == "Blueprints Button")
        {
            if (_page2Selected == false)
            {
                //_button1.transform.position = originalPosition1;
                //_button2.transform.position += _offset;
                //_button3.transform.position = originalPosition3;
                _page1Selected = false;
                _page2Selected = true;
                _page3Selected = false;
                _page1.SetActive(false);
                _page2.SetActive(true);
                _page3.SetActive(false);
            }
        } else if(buttonObj.name == "Help Button")
        {
            if (_page3Selected == false)
            {
                //_button1.transform.position = originalPosition1;
                //_button2.transform.position = originalPosition2;
                //_button3.transform.position += _offset;
                _page1Selected = false;
                _page2Selected = false;
                _page3Selected = true;
                _page1.SetActive(false);
                _page2.SetActive(false);
                _page3.SetActive(true);
            }
        }
    }

}
