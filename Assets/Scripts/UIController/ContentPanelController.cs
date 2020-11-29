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
    private GameObject btnACGroup;
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
    //判断当前是否是state在输入
    private bool isStateInput = false;
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

        InitCommonUI();
        InitStateUI();
        InitTransitionUI();

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
        reader.Close();
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
    #region UI初始化:Awake()
    private void InitCommonUI()
    {
        gameObject.SetActive(false);
        transform.Find("BottomGroup/BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        iptContent = transform.Find("BottomGroup/IptContent").GetComponent<InputField>();
        iptContent.onEndEdit.AddListener((value) =>
        {
            SetEntityContent(value);
        });

        btnACGroup = transform.Find("BtnACGroup").gameObject;
        btnACGroup.GetComponent<Button>().onClick.AddListener(() => btnACGroup.SetActive(false));
        btnACGroup.SetActive(false);

    }
    private void InitStateUI()
    {
        goState = transform.Find("BottomGroup/State").gameObject;
        txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

        goState.transform.Find("BtnAddAction").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowBtnACGroup(true);
        });
    }
    private void InitTransitionUI()
    {
        goTransition = transform.Find("BottomGroup/Transition").gameObject;
        txtTransitionTopic = goTransition.transform.Find("ImgLineTopic/TxtTransitionTopic").GetComponent<Text>();

        goTransition.transform.Find("BtnAddCondition").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowBtnACGroup(false);
        });
    }
    #endregion

    #region 公开的重载函数
    /// <summary>
    /// 由State的 √按钮 点击调用
    /// </summary>
    public void ShowContentPanel(StateEntity state)
    {
        SetSTUIObjectActive(true, state, null);

        txtStateName.text = $"State Name : \n{state.stateName}";
    }

    /// <summary>
    /// 由 BtnLine按钮 点击调用
    /// </summary>
    public void ShowContentPanel(TransitionEntity transition)
    {
        SetSTUIObjectActive(false, null, transition);

        StateEntity preState = null;
        StateEntity nextState = null;
        foreach (StateEntity state in Entities.Instance.listState)
        {
            if (transition.pre.Equals(state.goItemState))
                preState = state;
            if (transition.next.Equals(state.goItemState))
                nextState = state;
        }
        txtTransitionTopic.text = $"  src : {preState.stateName}\n" +
            $"dest : {nextState.stateName}";
    }

    #endregion

    #region 公共代码部分
    /// <summary>
    /// 设置 state和transition ui面板的显隐
    /// </summary>
    /// <param name="isState"></param>
    /// <param name="state"></param>
    /// <param name="transition"></param>
    private void SetSTUIObjectActive(bool isState, StateEntity state, TransitionEntity transition)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        goState.SetActive(isState);
        goTransition.SetActive(!isState);

        //2020-11-16 15:46:31
        isStateInput = isState;
        curtState = state;
        curtTransition = transition;
        iptContent.text = isState ? state.content : transition.content;
    }

    private void ShowBtnACGroup(bool isAction)
    {
        if (!btnACGroup.activeSelf)
            btnACGroup.SetActive(true);
        Transform imgBg = transform.Find("BtnACGroup/ImgBg");

        for (int i = 0; i < imgBg.childCount; i++)
        {
            Destroy(imgBg.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<string, string> item in isAction ? listAction : listCondition)
        {
            GameObject goBtnAC = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnAC"), Vector3.zero, Quaternion.identity, imgBg);
            goBtnAC.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            goBtnAC.GetComponent<Button>().onClick.AddListener(() =>
            {
                iptContent.text += item.Value + "\n";
                SetEntityContent(iptContent.text);
            });
        }
    }
    /// <summary>
    /// 设置Entities中的State OR Transition 的Content变量
    /// </summary>
    /// <param name="value"></param>
    private void SetEntityContent(string value)
    {
        if (isStateInput)
            curtState.content = value;
        else
            curtTransition.content = value;
    }
    #endregion
}
