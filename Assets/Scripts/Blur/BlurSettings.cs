using System;
using UnityEngine;

[Serializable]
public class BlurSettings
{
    //水平方向执行模糊
    [Range(0,0.4f)] public float horizontalBlur;
    //垂直方向执行模糊
    [Range(0,0.4f)] public float verticalBlur;
}