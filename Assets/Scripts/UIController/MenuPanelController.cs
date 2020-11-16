using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    private readonly List<KeyValuePair<string, string>> listAction =
        new List<KeyValuePair<string, string>>();
    private readonly List<KeyValuePair<string, string>> listCondition =
        new List<KeyValuePair<string, string>>();

    private void Awake()
    {
        gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        InitListByXmlFile();
    }
    public void ShowMenuPanel(bool isAction)
    {
        gameObject.SetActive(true);
        Transform imgBg = transform.Find("ImgBg");

        foreach (KeyValuePair<string,string> item in listAction)
        {
            GameObject btn = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnAC"), Vector3.zero, Quaternion.identity, imgBg);
            btn.transform.GetChild(0).GetComponent<Text>().text = item.Key;
        }
    }
    private void InitListByXmlFile()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };
        XmlReader reader = XmlReader.Create(GlobalVariable.Instance.PathXml, settings);
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
}
