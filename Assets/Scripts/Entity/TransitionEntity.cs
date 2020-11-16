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

    public string content = "";
}
