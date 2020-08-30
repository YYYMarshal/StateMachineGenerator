
/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   4:15
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\ItemState.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	ItemState
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	Created before the above time
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemState : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private struct PivotFromBorder
    {
        public float halfWidth;
        public float top;
        public float bottom;
    }
    private PivotFromBorder pivotFromBorder;
    private GameObject goSettingPanel;

    private InputField iptName;

    private GameObject planeLineGroup;

    private void Awake()
    {
        Rect rt = GetComponent<RectTransform>().rect;
        pivotFromBorder.halfWidth = rt.width / 2f;
        pivotFromBorder.top = rt.height * (1 - GetComponent<RectTransform>().pivot.y);
        pivotFromBorder.bottom = rt.height * GetComponent<RectTransform>().pivot.y;

        goSettingPanel = transform.parent.Find("SettingPanel").gameObject;

        iptName = transform.GetChild(1).GetComponent<InputField>();

        planeLineGroup = GameObject.Find("PlaneLineGroup");
    }
    private int GetCurtStateIndex()
    {
        for (int i = 0; i < GlobalVariable.lstItemState.Count; i++)
        {
            if (gameObject.Equals(GlobalVariable.lstItemState[i].goItemState))
            {
                return i;
            }
        }
        return -1;
    }
    private int GetCurtLineIndex()
    {
        for (int i = 0; i < GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].lstLine.Count; i++)
        {
            if (GlobalVariable.CurtRelated.currentLine.Equals(
                GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].lstLine[i].line))
            {
                return i;
            }
        }
        return -1;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Double click with the left mouse button, then open the setting panel.
        if (eventData.button == PointerEventData.InputButton.Left &&
            eventData.clickCount == 2)
            goSettingPanel.SetActive(true);

        //If right-click to the state image gameobject, start drawing ray.
        if (eventData.button == PointerEventData.InputButton.Right)
            InstantiateLine();

        if (eventData.button == PointerEventData.InputButton.Left &&
            GlobalVariable.CurtRelated.isStartPaint)
        {
            Debug.Log("YYY");
            GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].receivePos = transform.GetChild(3).position;
            Vector3 endPos = GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].receivePos;
            GlobalVariable.CurtRelated.currentLine.SetPosition(1, GetRayPoint(endPos));
            GlobalVariable.CurtRelated.isStartPaint = false;

            GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].
                lstLine[GlobalVariable.CurtRelated.curtLineIndex].endPos = endPos;
        }

    }
    private void InstantiateLine()
    {
        GlobalVariable.CurtRelated.isStartPaint = true;

        GlobalVariable.CurtRelated.currentLine = Instantiate(
            Resources.Load<GameObject>("Prefabs/Line"), Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        GlobalVariable.CurtRelated.currentLine.transform.SetParent(planeLineGroup.transform);

        GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].launchPos = transform.GetChild(2).position;
        Vector3 startPos = GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].launchPos;
        GlobalVariable.CurtRelated.currentLine.SetPosition(0, GetRayPoint(startPos));

        GlobalVariable.CurtRelated.curtStateIndex = GetCurtStateIndex();

        LineClass lineClass = new LineClass(GlobalVariable.CurtRelated.currentLine, startPos);
        lineClass.pre = gameObject;
        GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].
            lstLine.Add(lineClass);

        GlobalVariable.CurtRelated.curtLineIndex = GetCurtLineIndex();
    }
    //https://www.cnblogs.com/ylwshzh/p/4460915.html
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 targetPos;
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            //Transform the screen point to world point int rectangle
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTrans, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
            {
                targetPos = worldPoint;

                //Control state objects will not move out of the window
                if (worldPoint.x - pivotFromBorder.halfWidth < 0)
                    targetPos.x = pivotFromBorder.halfWidth;
                if (worldPoint.x + pivotFromBorder.halfWidth > Screen.width)
                    targetPos.x = Screen.width - pivotFromBorder.halfWidth;

                if (worldPoint.y - pivotFromBorder.bottom < 0)
                    targetPos.y = pivotFromBorder.bottom;
                if (worldPoint.y + pivotFromBorder.top > Screen.height)
                    targetPos.y = Screen.height - pivotFromBorder.top;
                rectTrans.position = targetPos;
            }

            OnStateImageDrag_Line();
        }
    }
    private void OnStateImageDrag_Line()
    {
        GlobalVariable.CurtRelated.curtStateIndex = GetCurtStateIndex();
        ItemStateClass itemStateClass = GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex];
        foreach (LineClass item in itemStateClass.lstLine)
        {
            item.line.SetPosition(0, GetRayPoint(transform.GetChild(2).position));
        }
        for (int i = 0; i < GlobalVariable.lstItemState.Count; i++)
        {
            for (int j = 0; j < GlobalVariable.lstItemState[i].lstLine.Count; j++)
            {
                itemStateClass.receivePos = transform.GetChild(3).position;
                if (itemStateClass.receivePos == GlobalVariable.lstItemState[i].lstLine[j].endPos)
                {
                    GlobalVariable.lstItemState[i].lstLine[j].endPos = itemStateClass.receivePos;
                    GlobalVariable.lstItemState[i].lstLine[j].line.SetPosition(1, GetRayPoint(itemStateClass.receivePos));
                }
            }
        }
    }
    private void Update()
    {
        if (GlobalVariable.CurtRelated.isStartPaint)
        {
            GlobalVariable.CurtRelated.currentLine.SetPosition(1, GetRayPoint(Input.mousePosition));
            //if (Input.GetMouseButtonDown(1))
            //    GlobalVariable.isStartPaint = false;
        }
        //if (Input.GetMouseButtonDown(0))
        //    GlobalVariable.isStartPaint = false;
    }

    private Vector3 GetRayPoint(Vector3 vector3)
    {
        Ray ray = Camera.main.ScreenPointToRay(vector3);
        bool isCollider = Physics.Raycast(ray, out RaycastHit hitInfo);
        if (isCollider)
            return hitInfo.point;
        return Vector3.zero;
    }

}
