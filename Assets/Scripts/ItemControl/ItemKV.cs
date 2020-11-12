using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemKV : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("IptValue").GetComponent<InputField>().onEndEdit.AddListener(IptValueOnEndEdit);
    }
    private void IptValueOnEndEdit(string str)
    {
        int spLI = GlobalVariable.Instance.curt.settingPanelLineIndex;
        int IndexItemCondition = transform.parent.parent.GetSiblingIndex();
        int IndexItemKV = transform.GetSiblingIndex();
        Debug.Log(IndexItemCondition + "      " + IndexItemKV);
        string s = Entities.Instance.listTransition[spLI].listItemConditionStr[IndexItemCondition][IndexItemKV + 1].Value;
        Debug.Log(s);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
