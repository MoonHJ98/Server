using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RunServer
{
#if UNITY_EDITOR
	[MenuItem("Tools/Run Server")]
	private static void Run()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
		BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

			BuildPipeline.BuildPlayer(
				GetScenePaths(),
				"Builds/Win64/Server/Server.exe",
				BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
	}
	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}
	static string[] GetScenePaths()
	{
		//string[] scenes = new string[EditorBuildSettings.scenes.Length];
		//for (int i = 0; i < scenes.Length; i++)
		//{
		//	scenes[i] = EditorBuildSettings.scenes[i].path;
		//}
		//return scenes;

		string[] scenes = new string[1];
		scenes[0] = "Assets/Scenes/Game.unity";

		return scenes;
	}

#endif
}
