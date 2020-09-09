
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
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    #region State Properties
    private GameObject goState;
    private Text txtStateName;
    private Dropdown ddlAction;
    #endregion

    #region Transition Properties
    private GameObject goTransition;
    private Text txtTransitionTopic;
    private Dropdown ddlCondition;
    #endregion

    List<List<KeyValuePair<string, string>>> lstXmlAction = new List<List<KeyValuePair<string, string>>>();
    List<List<KeyValuePair<string, string>>> lstXmlCondition = new List<List<KeyValuePair<string, string>>>();

    private void Awake()
    {
        gameObject.SetActive(false);
        SetTopMenuUI();
        SetStateUI();
        SetTransitionUI();

        InitDdlActionDdlCondition();

        void SetTopMenuUI()
        {
            transform.Find("TopMenu").Find("BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
            transform.Find("TopMenu").Find("BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        }
        void SetStateUI()
        {
            goState = transform.Find("State").gameObject;
            txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

            ddlAction = goState.transform.Find("DdlAction").GetComponent<Dropdown>();
            ddlAction.options.Clear();
            ddlAction.onValueChanged.AddListener((index) => ddlAction.gameObject.SetActive(false));
            ddlAction.gameObject.SetActive(false);

            goState.transform.Find("BtnActionGroup/BtnActionAdd").GetComponent<Button>().onClick.AddListener(() => ddlAction.gameObject.SetActive(true));
            goState.transform.Find("BtnActionGroup/BtnActionDel").GetComponent<Button>().onClick.AddListener(BtnActionDelOnClick);
        }
        void SetTransitionUI()
        {
            goTransition = transform.Find("Transition").gameObject;
            txtTransitionTopic = goTransition.transform.Find("ImgLineTopic/TxtTransitionTopic").GetComponent<Text>();

            ddlCondition = goTransition.transform.Find("DdlCondition").GetComponent<Dropdown>();
            ddlCondition.options.Clear();
            ddlCondition.onValueChanged.AddListener((index) =>
            {
                ddlCondition.gameObject.SetActive(false);
                Debug.Log(lstXmlCondition[4][2].Value);
            });
            ddlCondition.gameObject.SetActive(false);

            goTransition.transform.Find("BtnConditionGroup/BtnConditionAdd").GetComponent<Button>().onClick.AddListener(() => ddlCondition.gameObject.SetActive(true));
            goTransition.transform.Find("BtnConditionGroup/BtnConditionDel").GetComponent<Button>().onClick.AddListener(BtnConditionDelOnClick);
        }
    }
    private void InitDdlActionDdlCondition()
    {
        lstXmlAction = ReadXmlDoc(true);
        lstXmlCondition = ReadXmlDoc(false);
        lstXmlAction.ForEach(item => ddlAction.options.Add(new Dropdown.OptionData(item[0].Value)));
        lstXmlCondition.ForEach(item => ddlCondition.options.Add(new Dropdown.OptionData(item[0].Value)));

        List<List<KeyValuePair<string, string>>> ReadXmlDoc(bool isAction)
        {
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(GlobalVariable.Instance.PathXml);
            XmlNodeList nodLst = xmlFile.SelectSingleNode("YYYXB").ChildNodes;
            foreach (XmlElement elem in nodLst)
            {
                switch (elem.Name)
                {
                    case "Action":
                        List<KeyValuePair<string, string>> lstTempA = new List<KeyValuePair<string, string>>();
                        //Because elem.Attributes is XmlAttributeCollection Type, so cannot use ForEach()
                        foreach (XmlAttribute item in elem.Attributes)
                            lstTempA.Add(new KeyValuePair<string, string>(item.Name, item.InnerXml));
                        lstXmlAction.Add(lstTempA);
                        break;
                    case "Condition":
                        List<KeyValuePair<string, string>> lstTempB = new List<KeyValuePair<string, string>>();
                        foreach (XmlAttribute item in elem.Attributes)
                            lstTempB.Add(new KeyValuePair<string, string>(item.Name, item.InnerXml));
                        lstXmlCondition.Add(lstTempB);
                        break;
                }
            }
            return isAction ? lstXmlAction : lstXmlCondition;
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
    private void BtnHelpOnClick()
    {
        string url =
            "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
        Application.OpenURL(url);
    }
    #endregion

    #region State Methods
    private void BtnActionDelOnClick()
    {

    }
    public void SetSettingPanel(StateClass state)
    {
        SetStateTransitionUIShow(true);
        txtStateName.text = $"State Name : \n{state.iptName.text}";
    }
    #endregion

    #region Line Methods
    private void BtnConditionDelOnClick()
    {

    }
    public void SetSettingPanel(LineClass line)
    {
        SetStateTransitionUIShow(false);
        StateClass preStateClass = null;
        StateClass nextStateClass = null;
        foreach (StateClass stateClass in GlobalVariable.Instance.lstState)
        {
            if (line.pre.Equals(stateClass.goItemState))
                preStateClass = stateClass;
            if (line.next.Equals(stateClass.goItemState))
                nextStateClass = stateClass;
        }
        txtTransitionTopic.text = $"From : {preStateClass.iptName.text}\n" +
            $"    To : {nextStateClass.iptName.text}";
    }
    #endregion
    private void SetStateTransitionUIShow(bool isState)
    {
        goState.SetActive(isState);
        goTransition.SetActive(!isState);
    }
}
