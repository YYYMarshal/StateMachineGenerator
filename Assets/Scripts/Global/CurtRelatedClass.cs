using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurtRelatedClass
{
    public LineRenderer line = null;
    //检测LineRenderer是否开始绘制
    //2020-11-12 08:53:57  这个变量必须单一存在，不能放在ItemState类中当做其字段存在，因为会有多个ItemState类
    public bool isLineStartPaint = false;
    //ItemState中用的：当前的Line索引
    public int itemStateLineIndex = 0;
    //SettingPanel中用的：当前的Line/BtnLine索引
    public int settingPanelLineIndex = 0;
}
