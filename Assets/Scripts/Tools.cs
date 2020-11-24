using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tools : MonoBehaviour
{
    #region 单例
    private static readonly Lazy<Tools> lazy = new Lazy<Tools>(() => new Tools());
    public static Tools Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    #region 公开函数：Tip动画
    public void PlayTipAnimation(string content)
    {
        GameObject textTip = HierarchyObject.Instance.TextTip;
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
}
