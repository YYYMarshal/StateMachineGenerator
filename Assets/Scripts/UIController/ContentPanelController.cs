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
    private InputField iptContent;

    private StateEntity curtState;
    private TransitionEntity curtTransition;
    private bool isInputState = false;

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
        gameObject.SetActive(false);
        transform.Find("BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        iptContent = transform.Find("IptContent").GetComponent<InputField>();
        iptContent.onEndEdit.AddListener((value) =>
        {
            if (isInputState)
                curtState.content = value;
            else
                curtTransition.content = value;
        });

        SetStateUI();
        SetTransitionUI();

        #region 本地函数
        void SetStateUI()
        {
            goState = transform.Find("State").gameObject;
            txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

            goState.transform.Find("BtnAddAction").GetComponent<Button>().onClick.AddListener(() =>
            {
                HierarchyObject.Instance.MenuPanel.GetComponent<MenuPanelController>().ShowMenuPanel(true);
            });
        }
        void SetTransitionUI()
        {
            goTransition = transform.Find("Transition").gameObject;
            txtTransitionTopic = goTransition.transform.Find("ImgLineTopic/TxtTransitionTopic").GetComponent<Text>();

            goTransition.transform.Find("BtnAddCondition").GetComponent<Button>().onClick.AddListener(() =>
            {
                HierarchyObject.Instance.MenuPanel.GetComponent<MenuPanelController>().ShowMenuPanel(false);
            });
        }
        #endregion
    }
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

        int spLI = transition.line.transform.GetSiblingIndex();

        StateEntity preState = null;
        StateEntity nextState = null;
        foreach (StateEntity state in Entities.Instance.listState)
        {
            if (transition.pre.Equals(state.goItemState))
                preState = state;
            if (transition.next.Equals(state.goItemState))
                nextState = state;
        }
        txtTransitionTopic.text = $"From : {preState.iptName.text}\n" +
            $"    To : {nextState.iptName.text}";
        transition.topic = preState.iptName.text + "###" + nextState.iptName.text;
    }

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
    #endregion
}
