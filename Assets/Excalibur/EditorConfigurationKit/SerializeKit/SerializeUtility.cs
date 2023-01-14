using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using NPOI.Util;
using System.Linq;

namespace Excalibur
{
	public static class SerializeUtility
	{
		public static class FileExtension
		{
            public static string XLS = "xls";
            public static string XLSX = "xlsx";
            public static string JSON = "json";
            public static string XML = "xml";
            public static string TXT = "txt";
            public static string CS = "cs";
        }

        public static string FONT_PATH = "Assets/AssetPackages/Fonts/";
        public static string DATA_CONFIGURATION_NAME = "DataConfiguration.asset";
        public static string DEFAULT_DATA_CONFIFUGRATION_PATH = "Assets/Settings_SO/";
        public const string DATAHANDLER_PATH = "/Scripts/ConfigurationDatas/";
        public const string DATACLASS_PATH = "/Scripts/ConfigurationDatas/Datas/";

        public static void CheckPath(ref string path)
		{
			Replace(ref path);
			CheckPath(path);
        }

		public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

		public static void Replace(ref string path)
        {
            if (path.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }

		public static string[] GetFilesByPath(string path)
		{
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; ++i)
			{
				Replace(ref files[i]);
			}
			return files;
        }

        public static string[] GetFilesByPath(string path, string searchPattern)
        {
            string[] files = Directory.GetFiles(path, searchPattern);
            for (int i = 0; i < files.Length; ++i)
            {
                Replace(ref files[i]);
            }
            return files;
        }

        public static string[] GetFilesByPath(string path, string searchPattern, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(path, searchPattern, searchOption);
            for (int i = 0; i < files.Length; ++i)
            {
                Replace(ref files[i]);
            }
            return files;
        }

        public static string[] GetFilesNameOnly(string[] files)
		{
			string[] filesWithOutPath = new string[files.Length];
			for (int i = 0; i < files.Length; ++i)
			{
                filesWithOutPath[i] = GetFileNameOnly(files[i]);
			}
			return filesWithOutPath;
		}

		public static string GetFileNameOnly(string file)
		{
			int index = -1;
			for (int i = file.Length - 1; i >= 0; --i)
			{
				if (file[i].Equals(Path.DirectorySeparatorChar) || file[i].Equals(Path.AltDirectorySeparatorChar))
				{
					index = i;
					break;
				}
			}
			if (index > 0)
			{
				file = file.Substring(index + 1);
			}
			return file;
		}

		public static string AppendFileNameToPath(string path, string file)
		{
			Replace(ref path);
			if (!path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
			{
				path += Path.AltDirectorySeparatorChar;
			}
			string result = path + file;
			return result;
		}

		public static string[] FiltrateFilesByExtension(string[] files, string[] extension)
		{
            List<string> list = new List<string>();
			for (int i = 0; i < files.Length; ++i)
			{
				for (int j = 0; j < extension.Length; ++j)
                {
                    if (files[i].EndsWith(extension[j]))
					{
						list.Add(files[i]);
						break;
					}
                }
			}

			return list.ToArray();
		}

		public static void OpenFile(string path)
		{
			Process.Start(path);
		}

		public static FileStream OpenFile(string path, FileMode fileMode)
        {
            return File.Open(path, fileMode);
        }

        public static FileStream OpenFile(string path, FileMode fileMode, FileAccess fileAccess)
        {
            return File.Open(path, fileMode, fileAccess);
        }

        public static FileStream OpenFile(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			return File.Open(path, fileMode, fileAccess, fileShare);
        }

		public static void CreateDBHandler(string dbHandlerName)
		{
			string filePath = Application.dataPath + DATAHANDLER_PATH;
			CheckPath(filePath);
			filePath = string.Format("{0}{1}.{2}", filePath, dbHandlerName, FileExtension.CS);
            if (File.Exists(filePath))
            {
				File.Delete(filePath);
                //return;
            }
            using (StreamWriter stream = File.CreateText(filePath))
            {
				string readStr = "\tpublic static void ReadAllData()\n\t{";
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + DATACLASS_PATH);
				if (directoryInfo != null)
				{
					FileInfo[] files = directoryInfo.GetFiles();
					for (int i = 0; i < files.Length; i+=2)
					{
						FileInfo file = files[i];
						string filename = file.Name.Split('.')[0];
						readStr += string.Format("\n\t\t{0}.ReadData();", filename);
                    }
					readStr += "\n\t}";

                }
				string classStr = string.Format("using System;\nusing System.Collections.Generic;" +
                    "\n\npublic static class {0} \n{{\n{1}\n}}", dbHandlerName, readStr);
                stream.Write(classStr);
				stream.Close();
				stream.Dispose();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateDBScriptTemplate(string excelName, List<string> membersType, List<string> membersName)
		{
			string original = excelName;
			string filePath = Application.dataPath + DATACLASS_PATH;
            CheckPath(filePath);
            excelName = excelName.First().ToString().ToUpper() + excelName.Substring(1);
			string className = excelName + "Data";
			filePath = string.Format("{0}{1}.{2}", filePath, className, FileExtension.CS);
            if (File.Exists(filePath))
            {
				return;
				File.Delete(filePath);
			}
			using (StreamWriter stream = File.CreateText(filePath))
            {
				string memberStr = string.Empty;
				for (int i = 0; i < membersType.Count; ++i)
				{
					memberStr += string.Format("\tpublic {0} {1};\n", membersType[i], membersName[i]);
				}
                string classStr = string.Format("using System;\nusing System.Collections.Generic;\nusing Newtonsoft.Json;\nusing System.IO;\nusing Excalibur;" +
                    "\n\npublic class {0} : IItemData \n{{\n\tpublic static Dictionary<int, {1}> mTemplate = new Dictionary<int, {2}>();" +
                    "\n\n{3}\n\tpublic IItemData GetInfo(int id)\n\t{{\n\t\tif (mTemplate.TryGetValue(id, out {4} info))\n\t\t{{\n\t\t\treturn info;\n\t\t}}" +
                    "\n\t\treturn null;\n\t}}\n\n\tpublic static void ReadData()\n\t{{\n\t\tusing (StreamReader sr = File.OpenText(SerializeHelper.Instance.GetSavePath(GetFileName())))\n\t\t{{\n\t\t\tmTemplate = JsonConvert.DeserializeObject<Dictionary<int, {5}>>(sr.ReadToEnd().ToString());\n\t\t\tsr.Close();\n\t\t}}\n\t}}" +
                    "\n\n\tpublic static string GetFileName()\n\t{{\n\t\treturn \"{6}\";\n\t}}\n}}",
                    className, className, className, memberStr, className, className, original);
				stream.Write(classStr);
                stream.Close();
				stream.Dispose();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
