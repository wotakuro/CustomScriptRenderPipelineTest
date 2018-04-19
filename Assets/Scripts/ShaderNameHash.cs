using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shader Parameterを渡すためのPropertyIDを事前に持っておきます

public static class ShaderNameHash  {
    public static int MainTex;
    public static int RectValue;
    public static int ExpectedRect;
    public static int LightColor0;
    public static int WorldSpaceLightPos0;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        MainTex = Shader.PropertyToID("_MainTex");
        RectValue = Shader.PropertyToID("_RectValue");
        ExpectedRect = Shader.PropertyToID("_ExpectedRect");
        LightColor0 = Shader.PropertyToID("_LightColor0");
        WorldSpaceLightPos0 = Shader.PropertyToID("_WorldSpaceLightPos0");
    }

}
