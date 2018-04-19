# CustomScriptRenderPipelineTest
## About
このプロジェクトは 自作したScriptableRenderPipeline(SRP)のテスト用に作成しました。<br />
Unity 2018.1.0b13 で作成を行いました。<br />
このプロジェクト専用の Shader / 描画パスに特化した形でやっています。<br />

# 専用のSRPの効果
![alt text](docs/BeforeAfter.png)

## SRP使用のオン／オフを切り替えてみる
Menuの「Tools/SRPChanger」のチェックボックスの On/Offを切り替えることで、通常の描画パスと今回の専用描画パスを切り替えられます。<br />
![alt text](docs/SPRChanger.png)

未使用時は Batches 3733

![alt text](docs/SRPOff.png)

今回専用のRenderingパス時は Batches 31

![alt text](docs/CustomSRPOn.png)

専用に用意することで凄く軽くすることが出来ました。

## かるく解説
### 通常のパス
通常では、Transparentは描画の破綻を防ぐため「奥から手前」に描画をします。<br />
今回表示しているキャラクターや影は Spriteで表現していますので、α付のTexture表示になっています<br/>

レンダリングの破綻を防ぐため、下記の様に「奥から手前」を遵守します。<br/>
そのため、「影→キャラクター→影」という形でマテリアルを沢山切り替えながら描画をしますので、バッチ数は膨らみます。<br />

![alt text](docs/SRPOff_FrameDebugger.png)

### 今回カスタムの描画パス
今回は、下記手順で書き込んでいます。

1.先にキャラクターのαが0ではない箇所のみ、ZBufferに書き込み<br />
2.床や壁の描画<br />
3.キャラクターの実体をZTestで一致した所のみ描画するようにする<br />
4.最後に影を深度テストありで一気に書き込む<br />

このようにすることで、マテリアル切り替えを抑え描画するようにしています。<br />

FrameDebuggerで下記の様になっています。

![alt text](docs/CustomFrame1.png)

![alt text](docs/CustomFrame2.png)

![alt text](docs/CustomFrame3.png)


## ソース等

MyScriptableRenderPipeline.cs にてレンダリング関連の処理を行っています。<br />
ScriptableRenderPiepeline使用時には、「Tags { "LightMode" = "BasicPass"}」と言う形で描画用のパスタグを宣言する必要があります。<br />

そのため、今回はSRP使用のための専用Shaderを書きました。<br />
「Assets/Shaders/SRP」こちらが SRP使用時に利用する Shader<br />
<br />
通常時には、「Assets/Shaders/NonSRP」を利用して描画しています。
