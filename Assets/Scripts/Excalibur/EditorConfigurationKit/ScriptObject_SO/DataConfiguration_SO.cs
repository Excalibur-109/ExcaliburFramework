using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using static UnityEditor.Progress;

namespace Excalibur
{
	[Serializable]
	public class ImportedExcels
    {
        // <excelFileName, excelFilePath>
        public string filename;
		public string filepath;
		
		public ImportedExcels(string filename, string filepath)
		{
			this.filename = filename;
			this.filepath = filepath;
		}
	}

	//[CreateAssetMenu(fileName = "DataConfiguration", menuName = "Excalibur/Create Data Configuration")]
	[Serializable]
	public class DataConfiguration_SO : ScriptableObject
	{
        [SerializeField] List<ImportedExcels> importedExcels = new List<ImportedExcels>();

		public List<ImportedExcels> ImportedExcels => importedExcels;

        [SerializeField] List<string> serializeExcels = new List<string>(); // 与 importedExcels 的key对应

		public List<string> SerializeExcels => serializeExcels;

		public Action onReloadImportTree;
		public Action onReloadSerializeTree;

		public bool createDataHandlerScript = true;
		public bool createDataScript = false;

		string lastImportPath = string.Empty;
		public string LastImportPath
		{
			get
			{
				if (string.IsNullOrEmpty(lastImportPath))
				{
					lastImportPath = "Choose Path";
				}
				return lastImportPath;
			}
			set
            {
                lastImportPath = value;
				string[] files = SerializeUtility.GetFilesByPath(lastImportPath);
				files = SerializeUtility.FiltrateFilesByExtension(files, new string[1] { SerializeUtility.FileExtension.XLSX });
                string[] onlyFileNames = SerializeUtility.GetFilesNameOnly(files);

				for (int i = 0; i < onlyFileNames.Length; ++i)
				{
					ImportedExcels ie = importedExcels.Find(value => value.filename == onlyFileNames[i]);
					if (ie == null)
					{
						importedExcels.Add(new ImportedExcels(onlyFileNames[i], files[i]));
					}
				}
				onReloadImportTree?.Invoke();
            }
		}
		string lastExportLocalPath = string.Empty;
		public string LastExportLocalPath
		{
			get
			{
                if (string.IsNullOrEmpty(lastExportLocalPath))
                {
					switch (exportFormat)
					{
						case ExportFormat.xml:
							lastExportLocalPath = DEFAULT_XML_EXPORT_PATH;
							break;
						case ExportFormat.json:
							lastExportLocalPath = DEFAULT_JSON_EXPORT_PATH;
							break;
						case ExportFormat.txt:
							lastExportLocalPath = DEFAULT_TXT_EXPORT_PATH;
							break;
					}
				}
				return lastExportLocalPath;
			}
			set => lastExportLocalPath = value;
		}

		public string IP = "input ip";
		public string User = "input user";
		public string PassWord = "input password";

		public const string DEFAULT_XML_EXPORT_PATH = "/Assets/AssetPackages/DataBase/xml/";
		public const string DEFAULT_JSON_EXPORT_PATH = "/Assets/AssetPackages/DataBase/json/";
		public const string DEFAULT_TXT_EXPORT_PATH = "/Assets/AssetPackages/DataBase/txt/";

		[HideInInspector] public ExcelHandler excelHandler = ExcelHandler.ExcelDataReader_xlsx;
        [HideInInspector] public ExportFormat exportFormat = ExportFormat.xml;
        [HideInInspector] public SQLType sQLType = SQLType.Oracle;

		public ExportFormat ExportFormat
		{
			get { return exportFormat; }
			set
			{
				exportFormat = value;
				switch (exportFormat)
				{
					case ExportFormat.xml:
						lastExportLocalPath = DEFAULT_XML_EXPORT_PATH;
                        break;
					case ExportFormat.json:
                        lastExportLocalPath = DEFAULT_JSON_EXPORT_PATH;
                        break;
					case ExportFormat.txt:
                        lastExportLocalPath = DEFAULT_TXT_EXPORT_PATH;
                        break;
				}
			}
		}

		public string DBHandlerClassName = "ConfigurationHander";

		[Range(0f, 1f)]
		public float progress = 0f;

		public int ValidEndName
		{
			get
			{
				int max = 0;
				string newData = "newData";
				for (int i = 0; i < serializeExcels.Count; ++i)
				{
					string name = serializeExcels[i];
					if (name.Contains(newData))
					{
						name = name.Substring(newData.Length);
						if (int.TryParse(name, out int value))
						{
							if (max <= value)
							{
								max = value + 1;
							}
						}
					}
				}
				return max;
			}
		}

		public string GetImportedExcel(string excelName)
		{
			ImportedExcels ie = importedExcels.Find(value => value.filename == excelName);
			if (ie != null)
			{
				return ie.filepath;
			}
			return string.Empty;
		}

		/// <summary>
		/// 新增需要导入的excel文件
		/// </summary>
		public void AddImportExcel(string excelName, string excelPath)
        {
            ImportedExcels ie = importedExcels.Find(value => value.filename == excelName);

			if (ie != null)
			{
				ie.filepath = excelPath;
            }
			else
			{
				importedExcels.Add(new ImportedExcels(excelName, excelPath));
			}
			onReloadImportTree?.Invoke();
		}

		public bool RemoveImportExcel(string excelName)
        {
            ImportedExcels ie = importedExcels.Find(value => value.filename == excelName);

            if (ie != null)
            {
				importedExcels.Remove(ie);
                string name = excelName.Split('.')[0];
                if (serializeExcels.Contains(name))
                {
                    serializeExcels.Remove(name);
                    onReloadSerializeTree?.Invoke();
                    return true;
                }
            }

			return false;
		}

		/// <summary>
		/// 要导入到配置的excel表
		/// </summary>
		public void AddSerializeExcel(string excelName)
		{
			if (!serializeExcels.Contains(excelName))
			{
				serializeExcels.Add(excelName);
			}
		}

		public bool RemoveSerializeExcel(string excelName)
		{
			if (serializeExcels.Contains(excelName))
			{
				serializeExcels.Remove(excelName);
				return true;
			}
			return false;
		}

		public void RemoveSerializeExcel(IList<int> selection)
		{
			IList<string> remove = new List<string>();

			for (int i = 0; i < selection.Count; ++i)
			{
				remove.Add(serializeExcels[selection[i]]);
			}

			for (int i = 0; i < remove.Count; ++i)
			{
				serializeExcels.Remove(remove[i]);
			}
		}

		public void OnRenameSerilizeData(string originalName, string newname)
		{
			int index = serializeExcels.IndexOf(originalName);
			serializeExcels[index] = newname;
		}

		public bool OnRenameEndToCheck(string newName)
		{
			if (serializeExcels.Contains(newName))
			{
				return false;
			}
			return true;
		}

		public Dictionary<string, string> GetSerializePaths()
		{
			string error = string.Empty;
            Dictionary<string, string> dic = new Dictionary<string, string>();
			for (int i = 0; i < serializeExcels.Count; ++i)
			{
				string filename = serializeExcels[i] + "." + SerializeUtility.FileExtension.XLSX;

                ImportedExcels ie = importedExcels.Find(value => value.filename == filename);

                if (ie != null)
                {
                    dic.Add(serializeExcels[i], ie.filepath);
                }
				else
				{
					if (string.IsNullOrEmpty(error))
                    {
						error += "没在添加的文件路径中找到以下表：\n";

                    }
                    error += serializeExcels[i] + "\n";
                }
			}
			if (!string.IsNullOrEmpty(error))
			{
				EditorUtility.DisplayDialog("Error", error, "Confirm");
			}
			return dic;

        }

		public void SetProgress(float value)
		{
			progress = value;
		}

		public void ConnectToSql()
		{

		}

		public void ExportToLocalDirectly()
		{
            SerializeHelper.Instance.SerializeExcelToLocal();
        }

		public void ExportToLocalFromSQL()
        {
            SerializeUtility.CheckPath(ref lastExportLocalPath);
        }

		public void OnWindowsDestory()
		{
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
