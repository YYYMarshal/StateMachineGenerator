
/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   4:15
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\SettingPanel.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	SettingPanel
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	Created before the above time
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    private GameObject goBgMaskDropDown;
    private Dropdown ddlAction;
    private void Awake()
    {
        gameObject.SetActive(false);
        SetTopMenuUI();
        SetStateUI();

        InitDdlAction();

        void SetTopMenuUI()
        {
            transform.Find("TopMenu").Find("BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(BtnCloseSettingPanelOnClick);
            transform.Find("TopMenu").Find("BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        }
        void SetStateUI()
        {
            transform.Find("State").Find("BtnGroup").Find("BtnActionAdd").GetComponent<Button>().onClick.AddListener(BtnActionAddOnClick);
            //transform.Find("State/BtnGroup/BtnActionAdd").GetComponent<Button>().onClick.AddListener(BtnActionAddOnClick);
            transform.Find("State").Find("BtnGroup").Find("BtnActionDel").GetComponent<Button>().onClick.AddListener(BtnActionDelOnClick);
            goBgMaskDropDown = transform.Find("State").Find("BgMaskDropDown").gameObject;
            goBgMaskDropDown.SetActive(false);
            ddlAction = goBgMaskDropDown.transform.Find("DdlAction").GetComponent<Dropdown>();
            ddlAction.options.Clear();
            ddlAction.onValueChanged.AddListener(DdlActionOnValueChanged);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #region TopMenu
    private void BtnCloseSettingPanelOnClick()
    {
        gameObject.SetActive(false);
    }
    private void BtnHelpOnClick()
    {
        string url =
            "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
        Application.OpenURL(url);
    }
    #endregion

    #region State
    private void InitDdlAction()
    {
        List<string> lst = LoadXml();
        foreach (string item in lst)
        {
            ddlAction.options.Add(new Dropdown.OptionData(item));
        }
    }
    private List<string> LoadXml()
    {
        List<string> lst = new List<string>();
        XmlDocument xmlFile = new XmlDocument();
        xmlFile.Load(GlobalVariable.Instance.PathXml);
        XmlNodeList nodLst = xmlFile.SelectSingleNode("YYYXB").ChildNodes;
        foreach (XmlElement elem in nodLst)
        {
            switch (elem.Name)
            {
                case "Action":
                    lst.Add(elem.GetAttribute("type"));
                    break;
                case "Condition":
                    break;
            }
        }
        return lst;
    }
    private void DdlActionOnValueChanged(int index)
    {
        Debug.Log(index);
        goBgMaskDropDown.SetActive(false);
    }
    private void BtnActionAddOnClick()
    {
        goBgMaskDropDown.SetActive(true);
    }
    private void BtnActionDelOnClick()
    {

    }
    #endregion
}
