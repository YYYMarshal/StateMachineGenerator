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

    //2020-11-18 13:07:42
    //# 是需要的：防止在未给 pre 和 next 赋名称时，引起的报错
    //MenuPanelController.BtnExportOnClick().SetTransitionTopic().STListSort()
    public string topic = "#";
    public string content = "";
}
