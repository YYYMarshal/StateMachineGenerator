﻿/********************************************************************
	created:	2020/11/13
	created:	13:11:2020   10:00
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\Global\CurrentVariable.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\Global
	file base:	CurrentVariable
	file ext:	cs
	author:		YYYXB
	
	purpose:	当前的一些变量，比如需要在多个ItemState类中通用的 isLineStartDraw
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CurrentVariable
{
    #region 单例
    private static readonly Lazy<CurrentVariable> lazy = 
        new Lazy<CurrentVariable>(() => new CurrentVariable());
    public static CurrentVariable Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    //2020-11-12 08:53:57  这个变量必须单一存在，不能放在ItemState类中当做其字段存在，因为会有多个ItemState类
    /// <summary>
    /// 检测LineRenderer是否开始绘制
    /// </summary>
    public bool IsLineStartDraw = false;

    /// <summary>
    /// 选择的目标xml文件的路径
    /// </summary>
    public string TargetFileName = "";
}
