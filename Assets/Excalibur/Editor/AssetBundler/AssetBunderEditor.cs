using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;

namespace Excalibur
{
	public class AssetBunderEditor
	{
		[MenuItem("Excalibur/Asset Bundler", false, 2)]
		static void OpenAssetBundleWin()
		{
			AssetBundlerEditorWin win = EditorWindow.GetWindow<AssetBundlerEditorWin>("AssetBundler");
			win.Show();
        }
	}

	public class AssetBundlerEditorWin : EditorWindow
	{
		Vector2 minWinSize = new Vector2(609f, 579f);

		const float offset = 3f;
        Color rectColor = new Color(0.1764706f, 0.1764706f, 0.1764706f);

		private void OnEnable()
		{
			minSize = minWinSize;
		}

		private void OnGUI()
		{
			Rect rect = new Rect(offset, offset, position.width - offset * 2f, position.height - offset * 2f);
			EditorGUI.DrawRect(rect, Color.cyan);
		}
	}
}
