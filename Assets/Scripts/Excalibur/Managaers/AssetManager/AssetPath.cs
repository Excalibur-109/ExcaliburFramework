using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Excalibur
{
    public static class AssetPath
    {
        static readonly StringBuilder assetPathSB = new StringBuilder(128);

        const string EDITOR_ASSET_PATH = "Assets/AssetPackages/";

        public static string GetAssetPath(AssetType assetType, string name)
        {
            assetPathSB.Clear();
            assetPathSB.AppendFormat("{0}", EDITOR_ASSET_PATH);
            switch (assetType)
            {
                case AssetType.AT_FORM:
                    assetPathSB.AppendFormat("{0}{1}", "Prefabs/Form/" + name, ".prefab");
                    break;
                case AssetType.MainCamera:
                case AssetType.EventSystem:
                case AssetType.DirectionalLight:
                    assetPathSB.AppendFormat("{0}{1}", "GameDesired/" + name, ".prefab");
                    break;
            }

            return assetPathSB.ToString();
        }
    }
}
