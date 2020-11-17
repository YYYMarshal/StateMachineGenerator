using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class SettingPanelController : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        transform.Find("BtnGroup/BtnExport").GetComponent<Button>().onClick.AddListener(BtnExportOnClick);
        transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
    }

    #region 点击事件
    private void BtnExportOnClick()
    {
        XmlDocument xmlDoc = new XmlDocument();

        //XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
        //xmlDoc.AppendChild(xmlDec);

        XmlElement elemRoot = xmlDoc.CreateElement("CustomStateMachine");
        xmlDoc.AppendChild(elemRoot);

        XmlElement elemB = xmlDoc.CreateElement("StateMachine");
        elemRoot.AppendChild(elemB);

        XmlElement elemC = xmlDoc.CreateElement("StateMachine");
        elemC.SetAttribute("name", "StateMachineName");
        elemC.SetAttribute("sceneid", "1");
        elemB.AppendChild(elemC);

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
        GUIUtility.systemCopyBuffer = xmlDoc.InnerXml;
    }
    private void BtnHelpOnClick()
    {
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
}
