using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEditor.IMGUI.Controls;

namespace Excalibur
{
    public class DataConfigurationEditor
    {
        [MenuItem("Excalibur/Data Configuration", false, 1)]
        static void OpenDataConfigurationEditorWin()
        {
            DataConfigurationEditorWin win = EditorWindow.GetWindow<DataConfigurationEditorWin>("Data Configuration");
            win.Show();
        }
    }

    public class DataConfigurationEditorWin : EditorWindow
    {
        Vector2 winMinSize = new Vector2(800f, 450f);
        GUIStyle titleStyle;
        GUIStyle fontStyle;
        GUIStyle sqlLabelStyle;
        GUIStyle btnStyle;
        GUIStyle textStyle;
        Color rectColor = new Color(0.1764706f, 0.1764706f, 0.1764706f);
        Color tiffanyBlue = new Color(0.5019608f, 0.8196079f, 0.7843137f); // 蒂芙尼蓝
        Color lightBrown = new Color(1f, 0.8313726f, 0.6627451f); // 浅驼色

        const float offset = 2.5f;

        TreeViewState importedExcelsTreeState;
        ImportedExcelsList importedExcelsTree;

        TreeViewState dataToImportTreeState;
        SerializeExcelList dataToImportTree;

        public DataConfiguration_SO data_SO;

        private void OnEnable()
        {
            minSize = winMinSize;
            maxSize = winMinSize;
            position = new Rect(630f, 270f, minSize.x, minSize.y);

            string pathSO = SerializeUtility.DEFAULT_DATA_CONFIFUGRATION_PATH + SerializeUtility.DATA_CONFIGURATION_NAME;
            data_SO = SerializeHelper.LoadSetting();
            if (data_SO == null)
            {
                data_SO = CreateInstance<DataConfiguration_SO>();
                string path = Path.GetFullPath(".");
                path = path.Replace("\\", "/");
                path += "/" + SerializeUtility.DEFAULT_DATA_CONFIFUGRATION_PATH;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                AssetDatabase.CreateAsset(data_SO, pathSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            data_SO.SetProgress(0f);

            if (fontStyle == null)
            {
                fontStyle = new GUIStyle();
                Font font = AssetManager.Instance.LoadAsset<Font>(SerializeUtility.FONT_PATH, "CascadiaMono.ttf");
                if (font != null)
                {
                    fontStyle.font = font;
                }
                fontStyle.fontSize = 18;
                fontStyle.normal.textColor = lightBrown;
            }

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle();
                titleStyle.font = fontStyle.font;
                titleStyle.fontSize = 24;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.normal.textColor = tiffanyBlue;
            }

            if (sqlLabelStyle == null)
            {
                sqlLabelStyle = new GUIStyle() { fontSize = 14, normal = fontStyle.normal };
            }

            if (btnStyle == null)
            {
                btnStyle = new GUIStyle();
                btnStyle.font = fontStyle.font;
                btnStyle.fontSize = fontStyle.fontSize;
                btnStyle.alignment = TextAnchor.MiddleCenter;
                btnStyle.normal.textColor = tiffanyBlue;
                btnStyle.normal.background = Texture2D.linearGrayTexture;
                btnStyle.active.background = Texture2D.grayTexture;
                btnStyle.border = new RectOffset(1, 1, 1, 1);
            }

            if (textStyle == null)
            {
                textStyle = new GUIStyle();
                textStyle.font = fontStyle.font;
                textStyle.fontSize = fontStyle.fontSize;
                textStyle.alignment = TextAnchor.MiddleLeft;
                textStyle.normal.background = Texture2D.linearGrayTexture;
                textStyle.normal.textColor = tiffanyBlue;
                textStyle.border = new RectOffset(1, 1, 1, 1);
                textStyle.clipping = TextClipping.Clip;
            }

            if (importedExcelsTree == null)
            {
                importedExcelsTreeState = new TreeViewState();
                importedExcelsTree = new ImportedExcelsList(importedExcelsTreeState, this);
            }

            if (dataToImportTree == null)
            {
                dataToImportTreeState = new TreeViewState();
                dataToImportTree = new SerializeExcelList(dataToImportTreeState, this);
            }
        }

        private void OnGUI()
        {
            // top rectangle
            Rect top = new Rect(3f, offset, position.width - 5f, 30f);
            EditorGUI.DrawRect(top, rectColor);
            EditorGUI.LabelField(top, new GUIContent("Excalibur Data Configuration"), titleStyle);

            #region Left Rectangle
            // left rectangle
            Rect left = new Rect(3f, 35f, position.width * 0.5f - offset, position.height - 70f);
            EditorGUI.DrawRect(left, rectColor);

            Rect leftinleft = new Rect(left.x + offset, left.y + offset, left.width / 2f - offset, left.height - offset * 3f);
            EditorGUI.DrawRect(leftinleft, Color.gray);

            Rect leftTipRect = new Rect(leftinleft.x, leftinleft.y, leftinleft.width, 25f);
            EditorGUI.LabelField(leftTipRect, "Added Excels:", fontStyle);
            Rect importTreeRect = new Rect(leftinleft.x, leftTipRect.y + leftTipRect.height + offset, leftinleft.width,
                leftinleft.height - leftTipRect.height);
            importedExcelsTree.OnGUI(importTreeRect);

            Rect rightinleft = new Rect(leftinleft.x + leftinleft.width + offset, leftinleft.y, leftinleft.width - offset, leftinleft.height);
            EditorGUI.DrawRect(rightinleft, Color.gray);
            Rect importedTipRect = new Rect(rightinleft.x, rightinleft.y, rightinleft.width, 25f);
            EditorGUI.LabelField(importedTipRect, "Serialize Excels:", fontStyle);

            Rect toImportTreeRect = new Rect(rightinleft.x, importedTipRect.y + importedTipRect.height + offset, rightinleft.width,
                rightinleft.height - importedTipRect.height);
            dataToImportTree.OnGUI(toImportTreeRect);
            #endregion

            #region Right Rectangle
            // right rectangle
            Rect right = new Rect(position.width * 0.5f + offset, 35f, position.width * 0.5f - 5f, position.height - 70f);
            EditorGUI.DrawRect(right, rectColor);
            EditorGUI.LabelField(new Rect(right.x + offset, right.y + offset, right.width, 25f),
                new GUIContent("Import:"), fontStyle);

            if (GUI.Button(new Rect(right.x + offset, right.y + 30f, right.width - offset, 25f), new GUIContent("import excel"), btnStyle))
            {
                BrowseForImportFile();
            }

            Rect importPathRect = new Rect(right.x + offset, right.y + 60f, right.width - 100f, 25f);
            EditorGUI.TextField(importPathRect, data_SO.LastImportPath);//, textStyle);
            if (GUI.Button(new Rect(importPathRect.x + importPathRect.width + offset, importPathRect.y, right.width - importPathRect.width - offset * 2f, importPathRect.height), new GUIContent("add path"), btnStyle))
            {
                BrowseForImportPath();
            }

            Rect exportRect = new Rect(importPathRect.x, importPathRect.y + 30f, right.width - offset, importPathRect.height);
            EditorGUI.LabelField(exportRect, new GUIContent("Export:"), fontStyle);
            Rect exportPathRect = new Rect(exportRect.x, exportRect.y + 30f, importPathRect.width, importPathRect.height);
            EditorGUI.TextField(exportPathRect, data_SO.LastExportLocalPath);//, textStyle);
            if (GUI.Button(new Rect(exportPathRect.x + exportPathRect.width + offset, exportPathRect.y, right.width - exportPathRect.width - offset * 2f,
                exportPathRect.height), new GUIContent("out path"), btnStyle))
            {
                BrowseForExportPath();
            }

            Rect excelHandleRect = new Rect(exportPathRect.x, exportPathRect.y + 30f, exportRect.width, importPathRect.height);
            data_SO.excelHandler = (ExcelHandler)EditorGUI.EnumPopup(excelHandleRect, data_SO.excelHandler);
            Rect exportFormatRect = new Rect(exportPathRect.x, excelHandleRect.y + 20f, exportRect.width, importPathRect.height);
            data_SO.ExportFormat = (ExportFormat)EditorGUI.EnumPopup(exportFormatRect, data_SO.ExportFormat);
            Rect exportBtnRect = new Rect(exportPathRect.x, exportFormatRect.y + 20f, exportRect.width, importPathRect.height);
            if (GUI.Button(exportBtnRect, new GUIContent("export to local"), btnStyle))
            {
                ExportToLocal();
            }

            Rect sqlTypeRect = new Rect(exportBtnRect.x, exportBtnRect.y + exportBtnRect.height + 10f, exportBtnRect.width, exportBtnRect.height);
            data_SO.sQLType = (SQLType)EditorGUI.EnumPopup(sqlTypeRect, data_SO.sQLType);

            Rect ipLabelRect = new Rect(sqlTypeRect.x, sqlTypeRect.y + 30f, 50f, 30f);
            EditorGUI.LabelField(ipLabelRect, new GUIContent("IP:"), sqlLabelStyle);
            Rect ipRect = new Rect(ipLabelRect.x + 40f, ipLabelRect.y - 2.5f, right.width - 42.5f, 25f);
            data_SO.IP = EditorGUI.TextField(ipRect, data_SO.IP, textStyle);

            Rect userLabelRect = new Rect(ipLabelRect.x, ipLabelRect.y + 35f, 50f, 30f);
            EditorGUI.LabelField(userLabelRect, new GUIContent("User:"), sqlLabelStyle);
            Rect userRect = new Rect(ipRect.x, ipRect.y + 32.5f, ipRect.width, 25f);
            data_SO.User = EditorGUI.TextField(userRect, data_SO.User, textStyle);

            Rect passLabelRect = new Rect(ipLabelRect.x, userLabelRect.y + 35f, 50f, 30f);
            EditorGUI.LabelField(passLabelRect, new GUIContent("Pass:"), sqlLabelStyle);
            Rect passRect = new Rect(ipRect.x, userRect.y + 32.5f, ipRect.width, 25f);
            data_SO.PassWord = EditorGUI.TextField(passRect, data_SO.PassWord, textStyle);

            Rect exportToSqlRect = new Rect(exportBtnRect.x, passRect.y + 30f, right.width / 2f, right.height - passRect.y);
            if (GUI.Button(exportToSqlRect, new GUIContent("export to sql"), btnStyle))
            {
                ExportToSQL();
            }
            Rect importFromSqlRect = new Rect(exportToSqlRect.x + exportToSqlRect.width + offset / 2f, passRect.y + 30f, exportToSqlRect.width - offset * 2f, exportToSqlRect.height);
            if (GUI.Button(importFromSqlRect, new GUIContent("import from sql"), btnStyle))
            {
                ImportFormSql();
            }
            #endregion

            // bottom rectangle
            Rect bottom = new Rect(3f, left.height + left.y + offset, top.width, position.height - left.height - left.y - 5f);
            EditorGUI.DrawRect(bottom, rectColor);
            EditorGUI.ProgressBar(new Rect(bottom.x + 1f, bottom.y + 1f, bottom.width - 1f, bottom.height - 1f),
                data_SO.progress, $"{data_SO.progress * 100f}%");
        }

        // 导入文件
        private void BrowseForImportFile()
        {
            string file = EditorUtility.OpenFilePanel("Import File", data_SO.LastImportPath, SerializeUtility.FileExtension.XLSX);
            string filename = SerializeUtility.GetFileNameOnly(file);
            data_SO.AddImportExcel(filename, file);
        }

        // 导入路径
        private void BrowseForImportPath()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Import Folder", data_SO.LastImportPath, string.Empty);
            if (!string.IsNullOrEmpty(folderPath))
            {
                SerializeUtility.Replace(ref folderPath);
                data_SO.LastImportPath = folderPath;
            }
        }

        // 导出路径
        private void BrowseForExportPath()
        {
            Log.General("browse for export path.");
        }

        // 导出到本地
        private void ExportToLocal()
        {
            data_SO.ExportToLocalDirectly();
            Log.General("export to local.");
        }

        // 导出到数据库
        private void ExportToSQL()
        {
            Log.General("export to sql.");
        }

        // 从数据库导入
        private void ImportFormSql()
        {
            Log.General("import from sql.");
        }

        private void OnDestroy()
        {
            if (data_SO != null)
            {
                data_SO.OnWindowsDestory();
            }
        }
    }
}

