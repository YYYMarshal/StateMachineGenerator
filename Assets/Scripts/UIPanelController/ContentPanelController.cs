﻿/********************************************************************
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
    #region Hierarchy Object - Common
    private InputField iptContent;
    /// <summary>
    /// 点击添加某一项Action或Condition的面板
    /// </summary>
    private GameObject btnActionConditionGroupUI;
    #endregion

    #region Hierarchy Object - State
    private GameObject goState;
    private Text txtStateName;
    #endregion

    #region Hierarchy Object - Transition
    private GameObject goTransition;
    private Text txtTransitionTopic;
    #endregion

    #region BtnActionConditionGroupUI Releated
    /// <summary>
    /// key存储Action的type名称，value存储整行xml内容
    /// </summary>
    private readonly List<KeyValuePair<string, string>> listAction = new List<KeyValuePair<string, string>>();
    /// <summary>
    /// key存储Condition的type名称，value存储整行xml内容
    /// </summary>
    private readonly List<KeyValuePair<string, string>> listCondition = new List<KeyValuePair<string, string>>();
    #endregion

    #region Current
    private StateEntity currentState;
    private TransitionEntity currentTransition;
    /// <summary>
    /// 判断iptContent输入框当前输入的是否是state的content
    /// </summary>
    private bool isStateInput = false;
    #endregion

    private void Awake()
    {
        InitListByXmlFile();

        InitCommonUI();
        InitStateUI();
        InitTransitionUI();
    }
    #region INITIALIZATION
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
        XmlNodeList nodeList = doc.SelectSingleNode("YYYXB").ChildNodes;
        foreach (XmlElement elem in nodeList)
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
    private void InitCommonUI()
    {
        iptContent = transform.Find("BottomGroup/IptContent").GetComponent<InputField>();
        btnActionConditionGroupUI = transform.Find("BtnActionConditionGroupUI").gameObject;

        transform.Find("BottomGroup/BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
        iptContent.onEndEdit.AddListener((value) => SetEntityContent(value));
        btnActionConditionGroupUI.GetComponent<Button>().onClick.AddListener(() => btnActionConditionGroupUI.SetActive(false));

        gameObject.SetActive(false);
        transform.Find("BottomGroup").gameObject.SetActive(true);
        btnActionConditionGroupUI.SetActive(false);
    }
    private void InitStateUI()
    {
        goState = transform.Find("BottomGroup/State").gameObject;
        txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

        goState.transform.Find("BtnAddAction").GetComponent<Button>().onClick.AddListener(
            () => ShowBtnActionConditionGroupUI(true));
    }
    private void InitTransitionUI()
    {
        goTransition = transform.Find("BottomGroup/Transition").gameObject;
        txtTransitionTopic = goTransition.transform.Find("ImgTransitionTopic/TxtTransitionTopic").GetComponent<Text>();

        goTransition.transform.Find("BtnAddCondition").GetComponent<Button>().onClick.AddListener(
            () => ShowBtnActionConditionGroupUI(false));
    }
    #endregion

    #region PUBLIC OVERRIDE FUNCTION
    /// <summary>
    /// 由State物体的 选中（编辑）按钮 点击调用
    /// </summary>
    public void ShowContentPanel(StateEntity state)
    {
        SetStateTransitionUIActive(true, state, null);

        txtStateName.text = $"State Name : \n{state.stateName}";
    }
    /// <summary>
    /// 由 BtnLine按钮 点击调用
    /// </summary>
    public void ShowContentPanel(TransitionEntity transition)
    {
        SetStateTransitionUIActive(false, null, transition);

        StateEntity preState = null;
        StateEntity nextState = null;
        foreach (StateEntity state in Entities.Instance.ListState)
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

    #region REUSE FUNCTION
    /// <summary>
    /// 设置 State 和 Transition UI面板的显隐
    /// </summary>
    /// <param name="isState"></param>
    /// <param name="state"></param>
    /// <param name="transition"></param>
    private void SetStateTransitionUIActive(bool isState, StateEntity state, TransitionEntity transition)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        goState.SetActive(isState);
        goTransition.SetActive(!isState);

        //2020-11-16 15:46:31
        //切换 isStateInput 的状态，给 currentState 和 currentTransition 赋值相应变量值
        //并根据 isState 改变 iptContent 的 Text
        isStateInput = isState;
        currentState = state;
        currentTransition = transition;
        string str = isState ? state.content : transition.content;
        //iptContent.text = str.Replace(">\n", ">");
        iptContent.text = str.Replace(">", ">\n");
    }

    private void ShowBtnActionConditionGroupUI(bool isAction)
    {
        if (!btnActionConditionGroupUI.activeSelf)
            btnActionConditionGroupUI.SetActive(true);
        Transform imgBg = btnActionConditionGroupUI.transform.Find("ImgBg");
        for (int i = 0; i < imgBg.childCount; i++)
        {
            Destroy(imgBg.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<string, string> item in isAction ? listAction : listCondition)
        {
            GameObject goBtnActionCondition = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnActionCondition"), Vector3.zero, Quaternion.identity, imgBg);
            goBtnActionCondition.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            goBtnActionCondition.GetComponent<Button>().onClick.AddListener(() =>
            {
                iptContent.text += item.Value + "\n";
                SetEntityContent(iptContent.text);
            });
        }
    }
    /// <summary>
    /// 设置Entities中的State或Transition的content变量
    /// </summary>
    /// <param name="value"></param>
    private void SetEntityContent(string value)
    {
        if (isStateInput)
            currentState.content = value;
        else
            currentTransition.content = value;
    }
    #endregion
}
