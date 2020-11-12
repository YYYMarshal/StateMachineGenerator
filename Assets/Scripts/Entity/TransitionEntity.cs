using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionEntity
{
    //ItemState.OnPointerClick(PointerEventData eventData).CreateLine()
    public LineRenderer line = null;
    //ItemState.ControlBtnLine()
    public Button btnLine = null;
    //ItemState.OnPointerClick(PointerEventData eventData).CreateLine()
    public GameObject pre;
    //ItemState.OnPointerClick(PointerEventData eventData).EndDrawRay()
    public GameObject next;

    public bool isAnd;
    //每一个线(Transition)包含多个Condition，且每一个Condition包含多个 键值对类型的 属性
    public List<List<KeyValuePair<string, string>>> listItemConditionStr = new List<List<KeyValuePair<string, string>>>();

    public string content = "";
}
