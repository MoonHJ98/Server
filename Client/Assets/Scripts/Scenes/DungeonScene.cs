using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonScene : BaseScene
{
    UI_GameScene _sceneUI;
    protected override void Init()
    {
        base.Init();
		SceneType = Define.Scene.Dungeon;

		_sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

		Screen.SetResolution(640, 480, false);

    }

    public override void Clear()
    {
        
    }
}
