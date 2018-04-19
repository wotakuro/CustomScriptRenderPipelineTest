using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;


// プレイヤーループのカスタマイズ
// 参考URL https://www.patreon.com/posts/unity-2018-1-16336053
public class CustomPlayerLoop {

    [RuntimeInitializeOnLoadMethod]
    static  void Init()
    {
        var loopSystem = GenerateCustomLoop();
        //PlayerLoop.SetPlayerLoop( loopSystem );
    }

    private static PlayerLoopSystem GenerateCustomLoop()
    {
        // Note: this also resets the loop to its defalt state first.
        var playerLoop = PlayerLoop.GetDefaultPlayerLoop();

        for (int i = 0; i < playerLoop.subSystemList.Length; ++ i  )
        {
            var subSystem = playerLoop.subSystemList[i];
            // FixedUpdateの中身消します
            if (subSystem.type == typeof(FixedUpdate))
            {
                subSystem.subSystemList = CreateSubSystems();
            }
            // PreLateUpdateの中身消します
            if (subSystem.type == typeof(PreLateUpdate) )
            {
                subSystem.subSystemList = CreateSubSystems(typeof(UnityEngine.Experimental.PlayerLoop.PreLateUpdate.EndGraphicsJobsLate));
            }
            // Preupdateも大体削る
            else if (subSystem.type == typeof(PreUpdate))
            {
                subSystem.subSystemList = CreateSubSystems(typeof(UnityEngine.Experimental.PlayerLoop.PreUpdate.CheckTexFieldInput));
            }
            else
            {
                continue;
            }
            // 構造体なので上書きしないとセットされないです
            playerLoop.subSystemList[i] = subSystem;
        }
        return playerLoop;
    }

    private static PlayerLoopSystem[] CreateSubSystems(params System.Type[] types)
    {
        PlayerLoopSystem[] systems;
        if (types == null || types.Length == 0)
        {
            systems = new PlayerLoopSystem[0];
            return systems;
        }
        systems = new PlayerLoopSystem[types.Length];
        for (int i = 0; i < systems.Length; ++i)
        {
            systems[i] = new PlayerLoopSystem();
            systems[i].type = types[i];
        }
        return systems;
    }
}
