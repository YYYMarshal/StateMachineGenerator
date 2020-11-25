using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
public class DialogTest : MonoBehaviour
{
    /// <summary>
    /// 返回选择的文件的绝对路径
    /// </summary>
    /// <returns></returns>
    public string OpenSelectFileDialog()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        //openFileName.filter = "Excel文件(*.xlsx)\0*.xlsx";
        openFileName.filter = "xml文件(*.xml)\0*.xml";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = "窗口标题";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetSaveFileName(openFileName))
        {
            //Debug.Log(openFileName.file);
            //Debug.Log(openFileName.fileTitle);
            return openFileName.file;
        }
        return "";
    }
}