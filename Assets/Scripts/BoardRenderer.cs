using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// GPUインスタンスで一括描画をしたいので、専用Shader「BoardCharacter」を作成しました
/// 
/// ※SpriteRendererでは メッシュのUV座標が異なってしまうため、別Spriteになるとバッチングがうまくいきませんでした。
/// この問題を解決するため Shader側で、UV座標調整を行っております
[RequireComponent(typeof(MeshRenderer))]
public class BoardRenderer : MonoBehaviour {

    private Renderer rendererCache;
    public Rect rect = new Rect(0, 0, 1, 1);
    private MaterialPropertyBlock prop;

    void Awake() {
        rendererCache = this.GetComponent<Renderer>();
        prop = new MaterialPropertyBlock();
    }

    public void SetMaterial(Material material)
    {
        rendererCache.material = material;
    }

    public void SetRect(Rect r)
    {
        Vector4 val = new Vector4( r.x,r.y,r.width,r.height);
        prop.SetVector( ShaderNameHash.RectValue, val);
        rendererCache.SetPropertyBlock(prop);
        this.rect = r;
    }
}
