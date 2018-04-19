using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class MyScriptableRenderPipeline : RenderPipelineAsset
{
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new MyScriptableRenderPipelineInstance();
    }
}

/// このプロジェクト向けに作成した ScriptableRenderPipeline( SRP )です
public class MyScriptableRenderPipelineInstance : RenderPipeline
{
    private CullResults cull;
    private ScriptableCullingParameters cullingParams;
    private CommandBuffer cmd;
    private ShaderPassName zPrepass = new ShaderPassName("ZPrepass");
    private ShaderPassName basicPass = new ShaderPassName("BasicPass");


    private static MyScriptableRenderPipelineInstance instance;

    public static MyScriptableRenderPipelineInstance Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MyScriptableRenderPipelineInstance()
    {
        instance = this;
    }

    /// <summary>
    /// 実際の描画処理
    /// </summary>
    public override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        base.Render(context, cameras);
        if (cmd == null)
        {
            cmd = new CommandBuffer();
        }
        int idx = 0;
        foreach (var camera in cameras)
        {
            // Cullingします
            if (!CullResults.GetCullingParameters(camera, out cullingParams))
                continue;
            CullResults.Cull(ref cullingParams, context,ref cull);

            // カメラに関するShaderパラメーターをセットアップします
            context.SetupCameraProperties(camera);

            // 画面をクリアします
            cmd.Clear();
            cmd.ClearRenderTarget(true, true, Color.black,1.0f);
            context.ExecuteCommandBuffer(cmd);

            // Directional Lightの値を設定します
            SetUpDirectionalLightParam(cull.visibleLights);

            // キャラクターを　ZPrepassで描画します
            DrawCharacter(context, camera, zPrepass, SortFlags.CommonOpaque);
            // BGをBasicPassで描画します
            DrawBg(context, camera);
            // キャラクターをBasicPassで描画します
            DrawCharacter(context, camera, basicPass, SortFlags.OptimizeStateChanges);
            // 最後に影を描画します
            DrawShadow(context, camera);

            // 描画内容をコミットします
            context.Submit();
            ++idx;
        }
    }



    // 指定された　Passでキャラクターを描画します
    private void DrawCharacter(ScriptableRenderContext context, Camera camera, ShaderPassName pass,SortFlags sortFlags)
    {
        var settings = new DrawRendererSettings(camera, pass);
        settings.sorting.flags = sortFlags;

        var filterSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.transparent,
            layerMask = 1 << LayerDefine.CHARA
        };
        context.DrawRenderers(cull.visibleRenderers, ref settings, filterSettings);
    }

    // 指定された内容でBGを描画します
    private void DrawBg(ScriptableRenderContext context, Camera camera)
    {
        var settings = new DrawRendererSettings(camera, basicPass);
        settings.sorting.flags = SortFlags.CommonOpaque;

        var filterSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.opaque,
            layerMask = 1 << LayerDefine.BG
        };
        context.DrawRenderers(cull.visibleRenderers, ref settings, filterSettings);
    }
    // 指定された内容で影を描画します
    private void DrawShadow(ScriptableRenderContext context, Camera camera)
    {
        var settings = new DrawRendererSettings(camera, basicPass);
        settings.sorting.flags = SortFlags.CommonTransparent;

        var filterSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.transparent,
            layerMask = 1 << LayerDefine.SHADOW
        };
        context.DrawRenderers(cull.visibleRenderers, ref settings, filterSettings);
    }


    // Directional Lightの内容をShaderに反映させます
    private void SetUpDirectionalLightParam(List<VisibleLight> visibleLights)
    {
        if( visibleLights.Count <= 0 ){
            return;
        }
        foreach( var visibleLight in visibleLights)
        {
            if (visibleLight.lightType == LightType.Directional)
            {
                Vector4 dir = -visibleLight.localToWorld.GetColumn(2) ;
                Shader.SetGlobalVector(ShaderNameHash.LightColor0, visibleLight.finalColor);
                Shader.SetGlobalVector(ShaderNameHash.WorldSpaceLightPos0, new Vector4(dir.x,dir.y,dir.z,0.0f) );
                break;
            }
        }
    }

    // 破棄時の処理
    public override void Dispose()
    {
        base.Dispose();
        instance = null;
        if (cmd != null)
        {
            cmd.Dispose();
            cmd = null;
        }
    }
}
