/********************************************************************
	created:	2020/11/11
	created:	11:11:2020   15:45
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\ObjectAndVariable\GlobalObject.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\ObjectAndVariable
	file base:	GlobalObject
	file ext:	cs
	author:		YYYXB
	
	purpose:	该类存储的都是Hierarchy面板上一直存在的一些物体
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HierarchyObject
{
    #region 单例
    private static readonly Lazy<HierarchyObject> lazy = new Lazy<HierarchyObject>(() => new HierarchyObject());
    public static HierarchyObject Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    public GameObject GameManagerObject;

    public GameObject BtnLineGroup;
    public GameObject StateGroup;
    public GameObject PlaneLineGroup;

    public GameObject ContentPanel;

    public GameObject TextTip;
}
