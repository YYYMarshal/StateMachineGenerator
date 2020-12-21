using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class TopicInfoPanelController : MonoBehaviour
{
    private XmlDocument xmlDoc = null;

    private Transform defaultUI;
    private InputField iptSMName;
    private InputField iptSceneID;
    private void Awake()
    {
        gameObject.SetActive(false);

        gameObject.GetComponent<Button>().onClick.AddListener(
            () => gameObject.SetActive(false));

        defaultUI = transform.Find("SelctDefaultStateUI");
        iptSMName = transform.Find("TopicInfoEditUI/SMName/IptSMName").GetComponent<InputField>();
        iptSceneID = transform.Find("TopicInfoEditUI/SceneID/IptSceneID").GetComponent<InputField>();

        transform.Find("BtnSave").GetComponent<Button>().onClick.AddListener(BtnSaveOnClick);
    }
    /// <summary>
    /// 主要跟xml文件有关
    /// </summary>
    private void BtnSaveOnClick()
    {
        if (iptSMName.text.Trim() == "" || iptSceneID.text.Trim() == "")
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.IptValuteEmpty);
            return;
        }

        //2020-11-26 08:45:29
        //下面这样实现向同一个xml文件中多次写入 xml内容
        if (xmlDoc == null)
        {
            xmlDoc = CreateXmlDoc();
        }

        if (HaveSameStateMachineName(xmlDoc))
            return;

        XmlElement element = CreateItemTopic(xmlDoc);
        try
        {
            CreateItemContent(element, xmlDoc);
        }
        catch (System.Exception)
        {
            Debug.Log("Export Failure");
        }

        xmlDoc.Save(CurrentVariable.Instance.TargetFileName);

        //SelectSingleNode("XXX")：只能选择某结点的第一子级层级的结点
        GUIUtility.systemCopyBuffer = (xmlDoc.SelectSingleNode("AppData").SelectSingleNode("CustomStateMachine") as XmlElement).OuterXml;

        gameObject.SetActive(false);
    }
    #region ↑↑↑METHOD↑↑↑
    private XmlDocument CreateXmlDoc()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        //xmlDoc.Load(XmlReader.Create(GlobalVariable.Instance.TemplateXmlPath, settings));
        XmlReader reader = XmlReader.Create(CurrentVariable.Instance.TargetFileName, settings);
        xmlDoc.Load(reader);
        reader.Close();
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
    /// xml的每一个State
    /// </summary>
    /// <param name="elemC"></param>
    /// <param name="xmlDoc"></param>
    private void CreateItemContent(XmlElement elemC, XmlDocument xmlDoc)
    {
        string defaultStateName = "";
        for (int i = 0; i < defaultUI.childCount; i++)
        {
            if (defaultUI.GetChild(i).GetComponent<Toggle>().isOn)
            {
                defaultStateName = defaultUI.GetChild(i).Find("Label").GetComponent<Text>().text;
                break;
            }
        }
        for (int i = 0; i < Entities.Instance.listState.Count; i++)
        {
            StateEntity state = Entities.Instance.listState[i];
            XmlElement elem = xmlDoc.CreateElement("State");
            elem.SetAttribute("name", state.stateName);
            if (state.stateName.Trim() == defaultStateName)
                elem.SetAttribute("isDefaultState", "");
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
    private bool HaveSameStateMachineName(XmlDocument xmlDoc)
    {
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("AppData").SelectNodes("CustomStateMachine");
        foreach (XmlElement item in nodeList)
        {
            XmlElement elem = item.SelectSingleNode("StateMachine").SelectSingleNode("StateMachine") as XmlElement;
            if (iptSMName.text.Trim() == elem.GetAttribute("name"))
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.SameStateMachine);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 打开选择默认状态的UI
    /// </summary>
    private void OnEnable()
    {
        for (int i = 0; i < defaultUI.childCount; i++)
        {
            Destroy(defaultUI.GetChild(i).gameObject);
        }

        foreach (StateEntity state in Entities.Instance.listState)
        {
            GameObject toggleState = Instantiate(
                Resources.Load<GameObject>("Prefabs/ToggleState"), Vector3.zero, Quaternion.identity, defaultUI);
            toggleState.GetComponent<Toggle>().group = defaultUI.GetComponent<ToggleGroup>();
            toggleState.transform.Find("Label").GetComponent<Text>().text = state.stateName;
        }
    }
}
