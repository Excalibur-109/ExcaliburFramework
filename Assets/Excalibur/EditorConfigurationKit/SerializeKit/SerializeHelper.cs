using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Xml;
using Newtonsoft.Json.Linq;
using MiniExcelLibs;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using ExcelDataReader;
using System.Data;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;

namespace Excalibur
{
	public class SerializeHelper
	{
		private static SerializeHelper _instance;

		private static DataConfiguration_SO data_SO;

        public static SerializeHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SerializeHelper();
				}
				if (data_SO == null)
				{
                    data_SO = LoadSetting();
                }
				return _instance;
			}
		}

        public static DataConfiguration_SO LoadSetting()
        {
            return AssetDatabase.LoadAssetAtPath<DataConfiguration_SO>(SerializeUtility.DEFAULT_DATA_CONFIFUGRATION_PATH + SerializeUtility.DATA_CONFIGURATION_NAME);
        }

        public void SerializeExcelToLocal()
		{
			string projectPath = Path.GetFullPath(".");
			SerializeUtility.Replace(ref projectPath);
			projectPath += data_SO.LastExportLocalPath;
            SerializeUtility.CheckPath(ref projectPath);
            Dictionary<string, string> dic = data_SO.GetSerializePaths();
			float count = (float)dic.Count;
			int i = 1;
            foreach (var temp in dic)
            {
                SerializeToLocal(temp.Key, temp.Value);
                data_SO.SetProgress(i++ / count);
            }
            if (data_SO.createDataHandlerScript)
            {
                SerializeUtility.CreateDBHandler(data_SO.DBHandlerClassName);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

		private void SerializeToLocal(string filename, string fullPath)
        {
			using (FileStream stream = SerializeUtility.OpenFile(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))//, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Log.General(stream.ToString());
				switch (data_SO.excelHandler)
				{
					case ExcelHandler.NPOI_xlsx_xls:
						SerializeByNPOI(stream, filename);
						break;
					case ExcelHandler.MiniExcel_xlsx:
						SerializeByMini(fullPath, filename); 
                        break;
                    case ExcelHandler.ExcelDataReader_xlsx:
                        SerializeByExcelDataReader(stream, filename);
                        break;
                }
				stream.Close();
				stream.Dispose();
			}
        }

        private void SerializeByNPOI(FileStream stream, string filename)
        {
			List<string> membersName = new List<string>();
			List<string> membersType = new List<string>();
            IWorkbook wb = new XSSFWorkbook(stream);
			int sheetNumber = wb.NumberOfSheets;

			int i, j = 0, k;
            ICell cell;

            IRow row = wb.GetSheetAt(0).GetRow(j++);
            int lastCell = row.LastCellNum;
            for (i = 0; i < lastCell; ++i)
			{
				cell = row.GetCell(i);
                membersName.Add(cell.ToString());
            }

			row = wb.GetSheetAt(0).GetRow(j++);
			for (i = 0; i < lastCell; ++i)
			{
				membersType.Add(row.GetCell(i).ToString());
			}

			SerializeUtility.CreateDBScriptTemplate(filename, membersType, membersName);

			switch (data_SO.exportFormat)
			{
				case ExportFormat.xml:
					break;
				case ExportFormat.json:
     //               JObject jObect = new JObject();
     //               for (i = 0; i < sheetNumber; ++i)
     //               {
     //                   ISheet sheet = wb.GetSheetAt(i);
     //                   int lastRow = sheet.LastRowNum;
     //                   for (; j < lastRow; ++j)
     //                   {
					//		JObject childJObject = new JObject();
     //                       for (k = 0; k < lastCell; ++k)
     //                       {
     //                           cell = row.GetCell(k);
     //                           if (cell == null)
     //                           {
					//				cell = row.CreateCell(j);
					//				cell.SetCellValue(GetType(membersType[j]));
     //                           }
					//			jObect.Add(membersName[i], childJObject);
					//		}
     //                   }
					//	jObect.Add(sheet.GetRow(0).ToString(), jObect);
     //               }
     //               string savePath = Application.dataPath + data_SO.LastExportLocalPath;
					//string jsonFile = String.Format("{0}.{1}", filename, IOHelper.FileExtension.JSON);
					//savePath += savePath + jsonFile;
					//if (File.Exists(savePath))
					//{
					//	File.Delete(savePath);
					//}
					//using (StreamWriter sr = File.CreateText(savePath))
					//{
					//	sr.Write(jObect);
					//	sr.Close();
					//	sr.Dispose();
					//}
                    break;
				case ExportFormat.txt:
					break;
			}
        }

        private void SerializeByMini(string path, string filename)
        {
            using (var reader = MiniExcel.GetReader(path, true))
			{
				while (reader.Read())
				{
					for (int i = 0; i < reader.FieldCount; ++i)
					{
						var value = reader.GetValue(i);
					}
				}
			}
        }

        private void SerializeByExcelDataReader(FileStream stream, string filename)
        {
			using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
            {
                List<string> membersName = new List<string>();
                List<string> membersType = new List<string>();
                DataSet datas = reader.AsDataSet();
				int i, j, k;
				int sheetNum = datas.Tables.Count;
				int rows = datas.Tables[0].Rows.Count;
				int columns = datas.Tables[0].Columns.Count;

				for (i = 0; i < columns; ++i)
				{
					membersName.Add(datas.Tables[0].Rows[0][i].ToString());
                }

                for (i = 0; i < columns; ++i)
				{
					membersType.Add(datas.Tables[0].Rows[1][i].ToString());
                }

                SerializeUtility.CreateDBScriptTemplate(filename, membersType, membersName);

                switch (data_SO.ExportFormat)
                {
                    case ExportFormat.xml:
                        XmlDocument xml = new XmlDocument();
                        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
                        XmlElement root = xml.CreateElement(filename);
                        xml.AppendChild(root);
                        for (i = 0; i < sheetNum; ++i)
                        {
                            for (j = 2; j < rows; ++j)
                            {
                                filename = filename.First().ToString().ToUpper() + filename.Substring(1);
                                string className = filename + "Data";
                                string s = datas.Tables[i].Rows[j][0].ToString();
                                XmlElement element = xml.CreateElement(className + s);
                                root.AppendChild(element);
                                for (k = 0; k < columns; ++k)
                                {
                                    string str = datas.Tables[i].Rows[j][k].ToString();
                                    XmlElement child = xml.CreateElement(membersName[k]);
                                    child.InnerText = str;
                                    element.AppendChild(child);
                                }
                            }
                        }
                        SaveToPath(filename, xml);
                        break;
                    case ExportFormat.json:
                        JObject jObject = new JObject();
                        for (i = 0; i < sheetNum; ++i)
                        {
                            for (j = 2; j < rows; ++j)
                            {
                                JObject target = new JObject();
                                for (k = 0; k < columns; ++k)
                                {
                                    string str = datas.Tables[i].Rows[j][k].ToString();
                                    target.Add(membersName[k], str);
                                }
                                jObject.Add(datas.Tables[i].Rows[j][0].ToString(), target);
                            }
                        }
                        SaveToPath(filename, jObject);
                        break;
                    case ExportFormat.txt:
                        break;
                }
            }
        }

		public void SaveToPath(string filename, object datas)
        {
            string savePath = GetSavePath();
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            savePath += FileWithExtension(filename);
            if (datas is JObject)
            {
                using (StreamWriter sr = File.CreateText(savePath))
                {
                    sr.Write(datas);
                    sr.Close();
                    sr.Dispose();
                }
            }
            else if (datas is XmlDocument xml)
            {
                xml.Save(savePath);
            }
        }

        public string GetSavePath()
        {
            string savePath = Path.GetFullPath(".");
            SerializeUtility.Replace(ref savePath);
            savePath += data_SO.LastExportLocalPath;
            return savePath;
        }

        public string GetSavePath(string filename)
        {
            return GetSavePath() + FileWithExtension(filename);
        }

        public static string FileWithExtension(string filename)
        {
            string extension = string.Empty;
            switch (data_SO.ExportFormat)
            {
                case ExportFormat.xml:
                    extension = SerializeUtility.FileExtension.XML;
                    break;
                case ExportFormat.json:
                    extension = SerializeUtility.FileExtension.JSON;
                    break;
                case ExportFormat.txt:
                    extension = SerializeUtility.FileExtension.TXT;
                    break;
            }
            filename = String.Format("{0}.{1}", filename, extension);
            return filename;
        }

        public dynamic GetType(string str)
		{
			ValueGenre[] values = (ValueGenre[])Enum.GetValues(typeof(ValueGenre));

			string temp;
			for (ValueGenre i = 0; i < ValueGenre.Count; ++i)
            {
                temp = i.ToString().ToLower();
                if (temp == str)
                {
					switch (i)
					{
						case ValueGenre.Sbyte:
							return 0;
							return typeof(SByte);
						case ValueGenre.Byte:
                            return 0;
							return typeof(Byte);
                        case ValueGenre.Short:
                            return 0;
                            return typeof(Int16);
						case ValueGenre.Ushort:
                            return 0;
                            return typeof(UInt16);
                        case ValueGenre.Int:
                            return 0;
                            return typeof(Int32);
                        case ValueGenre.Uint:
                            return 0;
                            return typeof(UInt32);
                        case ValueGenre.Long:
                            return 0;
                            return typeof(Int64);
                        case ValueGenre.uLong:
                            return 0;
                            return typeof(UInt64);
						case ValueGenre.Bool:
                            return 0;
                            return typeof(Boolean);
						case ValueGenre.Float:
                            return 0;
                            return typeof(Single);
						case ValueGenre.Double:
                            return 0;
                            return typeof(Double);
						case ValueGenre.Char:
                            return '0';
                            return typeof(Char);
						case ValueGenre.String:
							return String.Empty;
                            return typeof(String);
					}
				}
            }

			return 0;
		}
	}
}
