/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   4:15
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\SettingPanel.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	SettingPanel
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	包含 关闭按钮 和 StateUI 以及 TransitionUI
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ContentPanelController : MonoBehaviour
{
    #region Hierarchy Object
    private InputField iptContent;
    private GameObject menuGroup;
    #endregion

    #region Menu Releated
    private readonly List<KeyValuePair<string, string>> listAction =
        new List<KeyValuePair<string, string>>();
    private readonly List<KeyValuePair<string, string>> listCondition =
        new List<KeyValuePair<string, string>>();
    #endregion

    #region Curt
    private StateEntity curtState;
    private TransitionEntity curtTransition;
    private bool isInputState = false;
    #endregion

    #region State Properties
    private GameObject goState;
    private Text txtStateName;
    #endregion

    #region Transition Properties
    private GameObject goTransition;
    private Text txtTransitionTopic;
    #endregion

    private void Awake()
    {
        InitListByXmlFile();

        SetCommonUI();
        SetStateUI();
        SetTransitionUI();

    }
    private void InitListByXmlFile()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };
        XmlReader reader = XmlReader.Create(GlobalVariable.Instance.ItemXmlPath, settings);
        XmlDocument doc = new XmlDocument();
        doc.Load(reader);
        XmlNodeList nodLst = doc.SelectSingleNode("YYYXB").ChildNodes;
        foreach (XmlElement elem in nodLst)
        {
            switch (elem.Name)
            {
                case "Action":
                    listAction.Add(
                        new KeyValuePair<string, string>(
                            elem.Attributes["type"].InnerXml, elem.OuterXml));
                    break;
                case "Condition":
                    listCondition.Add(
                        new KeyValuePair<string, string>(
                            elem.Attributes["type"].InnerXml, elem.OuterXml));
                    break;
            }
        }
    }
    #region Awake()：UI初始化
    private void SetCommonUI()
    {
        gameObject.SetActive(false);
        transform.Find("BottomGroup/BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        iptContent = transform.Find("BottomGroup/IptContent").GetComponent<InputField>();
        iptContent.onEndEdit.AddListener((value) =>
        {
            SetEntityContent(value);
        });

        menuGroup = transform.Find("MenuGroup").gameObject;
        menuGroup.GetComponent<Button>().onClick.AddListener(() => menuGroup.SetActive(false));
        menuGroup.SetActive(false);

    }
    private void SetStateUI()
    {
        goState = transform.Find("BottomGroup/State").gameObject;
        txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

        goState.transform.Find("BtnAddAction").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowMenuGroup(true);
        });
    }
    private void SetTransitionUI()
    {
        goTransition = transform.Find("BottomGroup/Transition").gameObject;
        txtTransitionTopic = goTransition.transform.Find("ImgLineTopic/TxtTransitionTopic").GetComponent<Text>();

        goTransition.transform.Find("BtnAddCondition").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowMenuGroup(false);
        });
    }
    #endregion
    #region 公开的重载函数
    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void ShowContentPanel(StateEntity state)
    {
        ShowUI_StateTransition(true, state, null);

        txtStateName.text = $"State Name : \n{state.iptName.text}";
    }

    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void ShowContentPanel(TransitionEntity transition)
    {
        ShowUI_StateTransition(false, null, transition);

        StateEntity preState = null;
        StateEntity nextState = null;
        foreach (StateEntity state in Entities.Instance.listState)
        {
            if (transition.pre.Equals(state.goItemState))
                preState = state;
            if (transition.next.Equals(state.goItemState))
                nextState = state;
        }
        txtTransitionTopic.text = $" src : {preState.iptName.text}\n" +
            $"dest : {nextState.iptName.text}";
    }

    #endregion

    #region 公共代码部分
    private void ShowUI_StateTransition(bool isState, StateEntity state, TransitionEntity transition)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        goState.SetActive(isState);
        goTransition.SetActive(!isState);

        //2020-11-16 15:46:31
        isInputState = isState;
        curtState = state;
        curtTransition = transition;
        iptContent.text = isState ? state.content : transition.content;
    }

    private void ShowMenuGroup(bool isAction)
    {
        if (!menuGroup.activeSelf)
            menuGroup.SetActive(true);
        Transform imgBg = transform.Find("MenuGroup/ImgBg");

        for (int i = 0; i < imgBg.childCount; i++)
        {
            Destroy(imgBg.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<string, string> item in isAction ? listAction : listCondition)
        {
            GameObject goBtnMenuItem = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnMenuItem"), Vector3.zero, Quaternion.identity, imgBg);
            goBtnMenuItem.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            //goBtnAC.GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
            goBtnMenuItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                //HierarchyObject.Instance.ContentPanel.GetComponent<ContentPanelController>().iptContent.text += item.Value + "\n";
                iptContent.text += item.Value + "\n";
                SetEntityContent(iptContent.text);
            });
        }
    }
    private void SetEntityContent(string value)
    {
        if (isInputState)
            curtState.content = value;
        else
            curtTransition.content = value;
    }
    #endregion
}
