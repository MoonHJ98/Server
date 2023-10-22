using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;
    protected override void Init()
    {
        base.Init();
		SceneType = Define.Scene.Game;

		Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();


        // 몬스터용 클라가 서버에 접속할 수 있도록 하는 코드
        if (SceneManager.sceneCountInBuildSettings == 1)
        {
            var scene = SceneManager.GetSceneAt(0);
			if(scene.name == "Game")
            {
                Managers.Network.ConnectToGame();
            }

		}
    }

    public override void Clear()
    {
        
    }
}
