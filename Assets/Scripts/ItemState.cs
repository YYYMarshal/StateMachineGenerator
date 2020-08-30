﻿
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
            GlobalVariable.CurtRelated.currentLine.SetPosition(1, GetRayPoint(transform.GetChild(3).position));
            GlobalVariable.CurtRelated.isStartPaint = false;
        }

    }
    private void InstantiateLine()
    {
        GlobalVariable.CurtRelated.isStartPaint = true;

        GlobalVariable.CurtRelated.currentLine = Instantiate(
            Resources.Load<GameObject>("Prefabs/Line"), Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        GlobalVariable.CurtRelated.currentLine.transform.SetParent(planeLineGroup.transform);

        Vector3 startPos = transform.GetChild(2).position;
        GlobalVariable.CurtRelated.currentLine.SetPosition(0, GetRayPoint(startPos));

        //TODO curtStateIndex的使用有待优化，在考虑删除
        for (int i = 0; i < GlobalVariable.lstItemState.Count; i++)
        {
            if (gameObject == GlobalVariable.lstItemState[i].goItemState)
            {
                GlobalVariable.CurtRelated.curtStateIndex = i;
                break;
            }
        }

        GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].
            lstLine.Add(new LineClass(GlobalVariable.CurtRelated.currentLine, startPos));
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

            if (gameObject.Equals(GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex]))
            {
                foreach (LineClass item in GlobalVariable.lstItemState[GlobalVariable.CurtRelated.curtStateIndex].lstLine)
                {
                    item.line.SetPosition(0, GetRayPoint(transform.GetChild(2).position));
                }
            }
            //if (GlobalVariable.CurtRelated.currentLine != null)
            //    GlobalVariable.CurtRelated.currentLine.SetPosition(0, GetRayPoint(transform.GetChild(2).position));
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
