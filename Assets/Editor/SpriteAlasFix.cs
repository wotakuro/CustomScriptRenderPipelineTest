using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

#if UNITY_2018_2_OR_NEWER
[InitializeOnLoad]
public class SpriteAlasFix  {
    static SpriteAlasFix()
    {
        Execute("Assets/Sprites/Misaki.spriteatlas");
        Execute("Assets/Sprites/Yuko.spriteatlas");
    }
    private static void Execute(string path)
    {
        var data = AssetDatabase.LoadAssetAtPath(path,typeof( SpriteAtlas) );

        SerializedObject serializedObject = new UnityEditor.SerializedObject(data);
        var packing = serializedObject.FindProperty("m_EditorData.packingSettings.enableTightPacking");
        var rotating = serializedObject.FindProperty("m_EditorData.packingSettings.enableRotation");

        bool originPacking = packing.boolValue;
        bool originRotating = rotating.boolValue;
        packing.boolValue = false;
        rotating.boolValue = false;
        serializedObject.ApplyModifiedProperties();
        if( originPacking || originRotating)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
