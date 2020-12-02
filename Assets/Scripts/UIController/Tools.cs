using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Tools : DialogTest
{
    private GameObject textTip;
    public static Tools Instance = null;
    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(true);
        textTip = transform.Find("TextTip").gameObject;
        textTip.SetActive(false);
    }

    #region 公开函数：Tip动画
    public void PlayTipAnimation(string content)
    {
        textTip.GetComponent<Text>().text = content;
        if (!textTip.activeSelf)
            textTip.SetActive(true);

        Animation animation = textTip.GetComponent<Animation>();
        if (animation.isPlaying)
        {
            animation.Stop();
            //始终保持：最后一次点击后，才开始倒计时关闭 TextTip游戏物体
            StopAllCoroutines();
        }
        animation.Play("TextTipAni");
        StartCoroutine(AnimationPlayDone(animation.GetClip("TextTipAni").length,
            () => textTip.SetActive(false)));
    }
    private IEnumerator AnimationPlayDone(float second, Action callback)
    {
        yield return new WaitForSeconds(second);
        callback?.Invoke();
    }
    #endregion

    #region 公开函数：文件选择
    /// <summary>
    /// 选择目标xml文件
    /// </summary>
    /// <returns></returns>
    public bool SelectXmlFile()
    {
        CurrentVariable.Instance.TargetFileName = Instance.OpenSelectFileDialog();
        if (CurrentVariable.Instance.TargetFileName == "")
        {
            Instance.PlayTipAnimation(GlobalVariable.Instance.NoSelectXml);
            return false;
        }
        return true;
    }
    #endregion
    
}
