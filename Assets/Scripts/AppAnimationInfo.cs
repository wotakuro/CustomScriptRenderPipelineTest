using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// スプライトのパラパラアニメーションの情報
public class AppAnimationInfo : ScriptableObject {
    public Sprite[] sprites;

    public const int AnglePatternNum = 8;

    private Rect[] preCalculateData;

    public Rect GetUvRect(int idx)
    {
        return preCalculateData[idx];
    }

    public Texture texture
    {
        get
        {
            return sprites[0].texture;
        }
    }

    public int Length
    {
        get { return preCalculateData.Length; }
    }

    public int animationLength
    {
        get { return preCalculateData.Length / AnglePatternNum; }
    }


    public void Initialize()
    {
        if (sprites == null || sprites.Length == 0) { return; }
        preCalculateData = new Rect[sprites.Length];
        float textureWidth = texture.width;
        float textureHeight = texture.height;
        for (int i = 0; i < sprites.Length; ++i)
        {
            var originRect = sprites[i].textureRect;

            preCalculateData[i] = new Rect(originRect.x / textureWidth, 
                originRect.y / textureHeight, 
                originRect.width / textureWidth, 
                originRect.height / textureHeight);
        }
    }

    /// <summary>
    ///  方向の取得を行います
    /// </summary>
    /// <param name="dir">カメラに対する向きのベクトルを指定</param>
    /// <returns> 0～7のいずれかで方向を返します</returns>
    public static int GetDirection(Vector3 dir)
    {
        float param1 = 0.84f;
        float param2 = 0.4f;

        dir.Normalize();
        if (dir.z > param1)
        {
            return 4;
        }
        else if (dir.z > param2)
        {
            if (dir.x > 0.0f) { return 3; }
            else { return 5; }
        }
        else if (dir.z > -param2)
        {
            if (dir.x > 0.0f) { return 2; }
            else { return 6; }
        }
        else if (dir.z > -param1)
        {
            if (dir.x > 0.0f) { return 1; }
            else { return 7; }
        }
        else
        {
            return 0;
        }
    }

}
