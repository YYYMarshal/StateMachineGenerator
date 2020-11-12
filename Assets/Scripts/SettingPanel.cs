
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
    private GameObject goContent;
    private Toggle toggleCondition;
    #endregion

    readonly List<List<KeyValuePair<string, string>>> lstXmlAction = new List<List<KeyValuePair<string, string>>>();
    readonly List<List<KeyValuePair<string, string>>> lstXmlCondition = new List<List<KeyValuePair<string, string>>>();

    private void Awake()
    {
        gameObject.SetActive(false);
        SetTopMenuUI();
        SetStateUI();
        SetTransitionUI();

        InitDdlActionDdlConditionByXml();

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
            ddlCondition.onValueChanged.AddListener(DdlConditionOnValueChanged);
            ddlCondition.gameObject.SetActive(false);

            goTransition.transform.Find("BtnConditionGroup/BtnConditionAdd").GetComponent<Button>().onClick.AddListener(() => ddlCondition.gameObject.SetActive(true));
            goTransition.transform.Find("BtnConditionGroup/BtnConditionDel").GetComponent<Button>().onClick.AddListener(BtnConditionDelOnClick);

            goContent = transform.Find("Transition/SV_Condition/Viewport/Content").gameObject;

            toggleCondition = transform.Find("Transition/ToggleCondition").GetComponent<Toggle>();
            toggleCondition.onValueChanged.AddListener((isChecked) =>
            {
                int spLI = GlobalVariable.Instance.curt.settingPanelLineIndex;
                Entities.Instance.listTransition[spLI].isAnd = isChecked;
            });
        }
    }
    private void InitDdlActionDdlConditionByXml()
    {
        ReadXmlDoc();
        lstXmlAction.ForEach(item => ddlAction.options.Add(new Dropdown.OptionData(item[0].Value)));
        lstXmlCondition.ForEach(item => ddlCondition.options.Add(new Dropdown.OptionData(item[0].Value)));

        void ReadXmlDoc()
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
    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void SetSettingPanel(StateEntity state)
    {
        ShowUI_StateTransition(true);
        txtStateName.text = $"State Name : \n{state.iptName.text}";
    }
    #endregion

    #region Line Methods
    /// <summary>
    /// DropDown组件的值改变的时候，同时创建ItemCondition物体
    /// </summary>
    private void DdlConditionOnValueChanged(int index)
    {
        ddlCondition.gameObject.SetActive(false);

        MyInstantiateItemCondition();
        void MyInstantiateItemCondition()
        {
            GameObject goItemCondition = Instantiate(Resources.Load<GameObject>("Prefabs/ItemCondition"), goContent.transform);

            int spLI = GlobalVariable.Instance.curt.settingPanelLineIndex;
            List<List<KeyValuePair<string, string>>> lstICStr = Entities.Instance.listTransition[spLI].listItemConditionStr;
            List<KeyValuePair<string, string>> lstTemp = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("type = ", lstXmlCondition[index][0].Value)
            };
            goItemCondition.transform.Find("TxtTypeKV").GetComponent<Text>().text = lstTemp[0].Key + lstTemp[0].Value;

            GameObject goKVGroup = goItemCondition.transform.Find("KVGroup").gameObject;
            //i从1开始，即从第二个属性开始，是因为第一个属性为type
            for (int i = 1; i < lstXmlCondition[index].Count; i++)
            {
                GameObject goItemKV = Instantiate(Resources.Load<GameObject>("Prefabs/ItemKV"), goKVGroup.transform);
                string key = $"{lstXmlCondition[index][i].Key} = ";
                string value = lstXmlCondition[index][i].Value;
                lstTemp.Add(new KeyValuePair<string, string>(key, value));
                goItemKV.transform.Find("TxtKey").GetComponent<Text>().text = key;
                goItemKV.transform.Find("IptValue").GetComponent<InputField>().text = value;
                if (goItemKV.GetComponent<ItemKV>() == null)
                    goItemKV.AddComponent<ItemKV>();
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(goContent.GetComponent<RectTransform>());
            lstICStr.Add(lstTemp);
        }
    }
    private void BtnConditionDelOnClick()
    {

    }
    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void SetSettingPanel(TransitionEntity line)
    {
        ShowUI_StateTransition(false);

        //int spLI = GlobalVariable.Instance.curt.settingPanelLineIndex;
        int spLI = line.line.transform.GetSiblingIndex();

        SetTopText();
        SetGoContent();

        toggleCondition.isOn = Entities.Instance.listTransition[spLI].isAnd;

        void SetTopText()
        {
            StateEntity preStateClass = null;
            StateEntity nextStateClass = null;
            foreach (StateEntity stateClass in Entities.Instance.listState)
            {
                if (line.pre.Equals(stateClass.goItemState))
                    preStateClass = stateClass;
                if (line.next.Equals(stateClass.goItemState))
                    nextStateClass = stateClass;
            }
            txtTransitionTopic.text = $"From : {preStateClass.iptName.text}\n" +
                $"    To : {nextStateClass.iptName.text}";
        }
        void SetGoContent()
        {
            for (int i = 0; i < goContent.transform.childCount; i++)
                Destroy(goContent.transform.GetChild(i).gameObject);

            MyInstantiateItemCondition();
            //下面的代码其实与 DdlConditionOnValueChanged(int index) 中生成ItemCondition的代码类似
            void MyInstantiateItemCondition()
            {
                List<List<KeyValuePair<string, string>>> lstICStr = Entities.Instance.listTransition[spLI].listItemConditionStr;
                for (int i = 0; i < lstICStr.Count; i++)
                {
                    GameObject goItemCondition = Instantiate(Resources.Load<GameObject>("Prefabs/ItemCondition"), goContent.transform);
                    goItemCondition.transform.Find("TxtTypeKV").GetComponent<Text>().text = lstICStr[i][0].Key + lstICStr[i][0].Value;
                    GameObject goKVGroup = goItemCondition.transform.Find("KVGroup").gameObject;
                    for (int j = 1; j < lstICStr[i].Count; j++)
                    {
                        GameObject goItemKV = Instantiate(Resources.Load<GameObject>("Prefabs/ItemKV"), goKVGroup.transform);
                        goItemKV.transform.Find("TxtKey").GetComponent<Text>().text = lstICStr[i][j].Key;
                        goItemKV.transform.Find("IptValue").GetComponent<InputField>().text = lstICStr[i][j].Value;
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(goContent.GetComponent<RectTransform>());
            }
        }
    }
    #endregion
    #region 公共代码部分
    private void ShowUI_StateTransition(bool isState)
    {
        goState.SetActive(isState);
        goTransition.SetActive(!isState);
    } 
    #endregion
}
