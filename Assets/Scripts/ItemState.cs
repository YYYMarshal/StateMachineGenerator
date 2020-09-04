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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        //Because GameObject.Find() can't find banned gameObject, so use Transform.Find(string n)
        goSettingPanel = transform.parent.Find("SettingPanel").gameObject;

        iptName = transform.Find("IptName").GetComponent<InputField>();
        transform.Find("BtnDelete").GetComponent<Button>().onClick.AddListener(BtnDeleteOnClick);


        planeLineGroup = GameObject.Find("PlaneLineGroup");
    }
    private void BtnDeleteOnClick()
    {
        GlobalVariable.lstState.Remove(GlobalVariable.lstState[GetCurtStateIndex()]);

        GameObject state = gameObject;
        //若在for循环中动态删除 GlobalVariable.lstLine 的元素，则会导致循环次数与预期不符，
        //因为 GlobalVariable.lstLine 的Count在减少
        //If the element of "GlobalVariable.lstLine" deleted dynamically in the for loop, The number of
        //cycles is not as expected, because "GlobalVariable.lstLine.Count" is decreasing.
        List<LineClass> lstLCTemp = new List<LineClass>();
        for (int i = 0; i < GlobalVariable.lstLine.Count; i++)
        {
            LineClass item = GlobalVariable.lstLine[i];
            int curtLineIndex = GetCurtLineIndex(item.line);
            if (state == item.pre)
            {
                lstLCTemp.Add(item);
                Destroy(planeLineGroup.transform.GetChild(curtLineIndex).gameObject);
            }
            else if (state == item.next)
            {
                lstLCTemp.Add(item);
                Destroy(planeLineGroup.transform.GetChild(curtLineIndex).gameObject);
            }
        }
        for (int i = 0; i < lstLCTemp.Count; i++)
            GlobalVariable.lstLine.Remove(lstLCTemp[i]);
        lstLCTemp.Clear();

        Destroy(gameObject);
    }
    private int GetCurtStateIndex()
    {
        for (int i = 0; i < GlobalVariable.lstState.Count; i++)
        {
            if (gameObject.Equals(GlobalVariable.lstState[i].goItemState))
            {
                return i;
            }
        }
        return -1;
    }
    private int GetCurtLineIndex(LineRenderer line)
    {
        for (int i = 0; i < GlobalVariable.lstLine.Count; i++)
        {
            if (line == GlobalVariable.lstLine[i].line)
            {
                return i;
            }
        }
        return -1;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Double click with the left mouse button, then open the setting panel.
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
            goSettingPanel.SetActive(true);

        //If right-click to the state image gameobject, start drawing ray.
        if (eventData.button == PointerEventData.InputButton.Right)
            InstantiateLine();

        //If left-click to the state image gameobject, end drawing ray.
        if (eventData.button == PointerEventData.InputButton.Left && GlobalVariable.curt.isStartPaint)
        {
            int curtLineIndex = GlobalVariable.curt.lineIndex;
            LineClass lineClass = GlobalVariable.lstLine[curtLineIndex];
            lineClass.line.SetPosition(1, GetRayPoint(transform.Find("EndPaintPos").position));
            GlobalVariable.curt.isStartPaint = false;

            SetEdgeColliderPoints(lineClass.line, lineClass.edge);
            lineClass.next = gameObject;

            bool isRepeated = false;
            //If the line is repeated, it will be deleted.
            for (int i = 0; i < GlobalVariable.lstLine.Count; i++)
            {
                if (curtLineIndex == i)
                    continue;
                if (GlobalVariable.lstLine[i].pre == lineClass.pre &&
                    GlobalVariable.lstLine[i].next == lineClass.next)
                {
                    isRepeated = true;
                    GlobalVariable.lstLine.Remove(lineClass);
                    Destroy(planeLineGroup.transform.GetChild(curtLineIndex).gameObject);

                    //GlobalVariable.curt.line = null;
                    //GlobalVariable.curt.lineIndex = -1;

                    //2020-9-2 12:40:01
                    //break is necessary!!!
                    break;
                }
            }
            //If the line is not repeated, it is judged whether it is self jump. If so, delete.
            if (!isRepeated && lineClass.pre == lineClass.next)
            {
                GlobalVariable.lstLine.Remove(lineClass);
                Destroy(planeLineGroup.transform.GetChild(curtLineIndex).gameObject);
            }
        }

    }
    private void InstantiateLine()
    {
        LineRenderer lineTemp = Instantiate(
            Resources.Load<GameObject>("Prefabs/Line"), Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        lineTemp.transform.SetParent(planeLineGroup.transform);

        LineClass lineItem = new LineClass()
        {
            line = lineTemp,
            edge = lineTemp.GetComponent<EdgeCollider2D>(),
            pre = gameObject
        };

        lineItem.line.SetPosition(0, GetRayPoint(transform.Find("StartPaintPos").position));

        GlobalVariable.lstLine.Add(lineItem);

        GlobalVariable.curt.line = lineTemp;
        GlobalVariable.curt.isStartPaint = true;
        GlobalVariable.curt.lineIndex = GetCurtLineIndex(lineTemp);
    }
    //https://www.cnblogs.com/ylwshzh/p/4460915.html
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 targetPos;
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            targetPos = Input.mousePosition;

            DragLimit();
            //Control state objects will not move out of the window
            void DragLimit()
            {
                if (Input.mousePosition.x - pivotFromBorder.halfWidth < 0)
                    targetPos.x = pivotFromBorder.halfWidth;
                if (Input.mousePosition.x + pivotFromBorder.halfWidth > Screen.width)
                    targetPos.x = Screen.width - pivotFromBorder.halfWidth;

                if (Input.mousePosition.y - pivotFromBorder.bottom < 0)
                    targetPos.y = pivotFromBorder.bottom;
                if (Input.mousePosition.y + pivotFromBorder.top > Screen.height)
                    targetPos.y = Screen.height - pivotFromBorder.top;
            }
            rectTrans.position = targetPos;

            OnStateImageDrag_LineControl();
            void OnStateImageDrag_LineControl()
            {
                GameObject state = gameObject;
                foreach (LineClass item in GlobalVariable.lstLine)
                {
                    if (state == item.pre)
                        item.line.SetPosition(0, GetRayPoint(transform.Find("StartPaintPos").position));
                    if (state == item.next)
                        item.line.SetPosition(1, GetRayPoint(transform.Find("EndPaintPos").position));
                }
            }
        }
    }
    private void Update()
    {
        if (GlobalVariable.curt.isStartPaint)
        {
            GlobalVariable.curt.line.SetPosition(1, GetRayPoint(Input.mousePosition));
        }
    }

    private Vector3 GetRayPoint(Vector3 vector3)
    {
        Ray ray = Camera.main.ScreenPointToRay(vector3);
        bool isCollider = Physics.Raycast(ray, out RaycastHit hitInfo);
        if (isCollider)
            return hitInfo.point;
        return Vector3.zero;
    }

    private void SetEdgeColliderPoints(LineRenderer line, EdgeCollider2D edge)
    {
        for (int i = 0; i < line.positionCount; i++)
            edge.points[i] = line.GetPosition(i);
    }
}
