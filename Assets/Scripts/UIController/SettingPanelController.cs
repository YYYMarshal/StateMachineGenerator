using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class SettingPanelController : MonoBehaviour
{
    private GameObject btnGroup;
    private GameObject topicInfo;

    private InputField iptSMName;
    private InputField iptSceneID;
    private void Awake()
    {
        FindObject();
        ObjectEvent();
        InitObjectActive();
    }
    #region Method:Awake()
    private void FindObject()
    {
        btnGroup = transform.Find("BtnGroup").gameObject;
        topicInfo = transform.Find("TopicInfo").gameObject;

        iptSMName = transform.Find("TopicInfo/TopicMenu/SMName/IptSMName").GetComponent<InputField>();
        iptSceneID = transform.Find("TopicInfo/TopicMenu/SceneID/IptSceneID").GetComponent<InputField>();
    }
    private void ObjectEvent()
    {
        transform.Find("BtnGroup/BtnExport").GetComponent<Button>().onClick.AddListener(BtnExportOnClick);
        transform.Find("BtnGroup/BtnNew").GetComponent<Button>().onClick.AddListener(BtnNewOnClick);
        transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);

        transform.Find("TopicInfo/BtnOK").GetComponent<Button>().onClick.AddListener(BtnOKOnClick);
    }
    private void InitObjectActive()
    {
        gameObject.SetActive(false);
        SetOneTwoActive(true);
    }

    #endregion

    #region BtnGroup
    private bool isFirst = true;
    private string destFileName = "";
    private void BtnExportOnClick()
    {
        if (!CheckStateName())
        {
            return;
        }
        SetOneTwoActive(false);
        ShowSelctDefaultStateMenu();
    }
    #region Method:BtnExportOnClick()
    /// <summary>
    /// 检查所有状态的名称是否为空
    /// </summary>
    /// <returns></returns>
    private bool CheckStateName()
    {
        if (Entities.Instance.listState.Count == 0)
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoState);
            return false;
        }
        foreach (StateEntity state in Entities.Instance.listState)
        {
            if (state.goItemState.transform.Find("IptName").GetComponent<InputField>().text.Trim() == "")
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.StateNameEmpty);
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 打开选择默认状态的UI
    /// </summary>
    private void ShowSelctDefaultStateMenu()
    {
        Transform imgBg = transform.Find("TopicInfo/SelctDefaultStateMenu/ImgBg");

        for (int i = 0; i < imgBg.childCount; i++)
        {
            Destroy(imgBg.GetChild(i).gameObject);
        }

        foreach (StateEntity state in Entities.Instance.listState)
        {
            GameObject toggleState = Instantiate(
                Resources.Load<GameObject>("Prefabs/ToggleState"), Vector3.zero, Quaternion.identity, imgBg);
            toggleState.GetComponent<Toggle>().group = imgBg.GetComponent<ToggleGroup>();
            toggleState.transform.Find("Label").GetComponent<Text>().text = state.iptName.text;
        }
    }
    #endregion

    private void BtnNewOnClick()
    {
        if (HierarchyObject.Instance.ContentPanel.activeSelf)
            HierarchyObject.Instance.ContentPanel.SetActive(false);

        Entities.Instance.listState.Clear();
        Entities.Instance.listTransition.Clear();

        DestroyAllChilds(HierarchyObject.Instance.StateGroup);
        DestroyAllChilds(HierarchyObject.Instance.BtnLineGroup);
        DestroyAllChilds(HierarchyObject.Instance.PlaneLineGroup);

        gameObject.SetActive(false);

        void DestroyAllChilds(GameObject go)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Destroy(go.transform.GetChild(i).gameObject);
            }
        }
    }
    private void BtnHelpOnClick()
    {
        gameObject.SetActive(false);

        string url =
            "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
        Application.OpenURL(url);
    }
    private void BtnExitOnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region TopicInfo
    private void BtnOKOnClick()
    {
        if (iptSMName.text.Trim() == "" || iptSceneID.text.Trim() == "")
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.IptValuteEmpty);
            return;
        }
        if (!SelectXmlFile())
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoSelectXml);
            return;
        }

        SetTransitionTopic();

        XmlDocument xmlDoc = CreateXmlDoc();
        XmlElement element = CreateItemTopic(xmlDoc);
        try
        {
            CreateItemContent(element, xmlDoc);
        }
        catch (System.Exception)
        {
            Debug.Log("Export Failure");
        }

        xmlDoc.Save(destFileName);

        //SelectSingleNode("XXX")：只能选择某结点的第一子级层级的结点
        GUIUtility.systemCopyBuffer = (xmlDoc.SelectSingleNode("AppData").SelectSingleNode("CustomStateMachine") as XmlElement).OuterXml;

        SetOneTwoActive(true);
        gameObject.SetActive(false);

    }
    #region Method:BtnOKOnClick()
    private void SetTransitionTopic()
    {
        foreach (TransitionEntity transition in Entities.Instance.listTransition)
        {
            transition.topic =
                transition.pre.transform.Find("IptName").GetComponent<InputField>().text + "#" +
                transition.next.transform.Find("IptName").GetComponent<InputField>().text;
        }
    }
    /// <summary>
    /// 选择目标xml文件
    /// </summary>
    /// <returns></returns>
    private bool SelectXmlFile()
    {
        if (isFirst)
        {
            destFileName = gameObject.GetComponent<DialogTest>().OpenSelectFileDialog();
            if (destFileName == "")
                return false;
            File.Copy(GlobalVariable.Instance.SourceXmlPath, destFileName, true);
            isFirst = false;
        }
        return true;
    }
    #region 创建Xml
    private XmlDocument CreateXmlDoc()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(XmlReader.Create(GlobalVariable.Instance.SourceXmlPath, settings));
        return xmlDoc;
    }
    /// <summary>
    /// AppData/CustomStateMachine/StateMachine/StateMachine/
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns></returns>
    private XmlElement CreateItemTopic(XmlDocument xmlDoc)
    {
        XmlElement elemAppdata = xmlDoc.SelectSingleNode("AppData") as XmlElement;

        XmlElement elemA = xmlDoc.CreateElement("CustomStateMachine");
        elemAppdata.AppendChild(elemA);

        XmlElement elemB = xmlDoc.CreateElement("StateMachine");
        elemA.AppendChild(elemB);

        XmlElement elemC = xmlDoc.CreateElement("StateMachine");
        elemC.SetAttribute("name", iptSMName.text);
        elemC.SetAttribute("sceneid", iptSceneID.text);
        elemB.AppendChild(elemC);

        return elemC;
    }
    /// <summary>
    /// 每一个State
    /// </summary>
    /// <param name="elemC"></param>
    /// <param name="xmlDoc"></param>
    private void CreateItemContent(XmlElement elemC, XmlDocument xmlDoc)
    {
        foreach (StateEntity state in Entities.Instance.listState)
        {
            XmlElement elem = xmlDoc.CreateElement("State");
            elem.SetAttribute("name", state.iptName.text);
            elem.InnerXml = state.content;
            elemC.AppendChild(elem);
        }
        foreach (TransitionEntity transition in Entities.Instance.listTransition)
        {
            XmlElement elem = xmlDoc.CreateElement("Transition");
            string[] strs = transition.topic.Split('#');
            elem.SetAttribute("src", strs[0]);
            elem.SetAttribute("dest", strs[1]);
            elem.InnerXml = transition.content;
            elemC.AppendChild(elem);
        }
    }
    #endregion
    #endregion

    #endregion
    private void OnEnable()
    {
        SetOneTwoActive(true);
    }
    /// <summary>
    /// 选择显示 BtnGroup 还是 TopicInfo
    /// </summary>
    /// <param name="isBtnGroupShow"></param>
    private void SetOneTwoActive(bool isBtnGroupShow)
    {
        btnGroup.SetActive(isBtnGroupShow);
        topicInfo.SetActive(!isBtnGroupShow);
    }
}
