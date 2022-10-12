using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using UnityEditor.IMGUI.Controls;
using static UnityEditor.Progress;
using System.IO;
using Newtonsoft.Json;

public class Test : MonoBehaviour
{
    [SerializeField]
    GUIStyle style;

    [SerializeField]
    TreeViewState TreeViewState;

    int i = 0x9;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Log.General(i.ToString());
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            read();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Image d = transform.GetChildComponentRecursively<Image>("test");
            if (d != null)
            {
                Log.General(d);
            }
            else
            {
                Log.General("ц╩сп");
            }
        }
    }

    void read()
    {
        //ConfigurationHander.ReadAllData();

        //string path = "F:/Gitee/Excalibur/Assets/AssetPackages/DataBase/json/test.json";
        string path = "F:/Gitee/Excalibur/Assets/AssetPackages/DataBase/xml/test.xml";
        System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
        xml.Load(path);
        string s = xml.ToString();
        //Dictionary<string, TestData> dic = JsonConvert.DeserializeObject<Dictionary<string, TestData>>(s);
        //List<TestData> list = JsonConvert.DeserializeObject<List<TestData>>(xml.ToString());
        //for (int i = 0; i < list.Count; ++i)
        //{
        //    TestData.mTemplate.Add(list[i].id, list[i]);
        //}
        return;
        using (StreamReader sr = File.OpenText(path))
        {
            string str = sr.ReadToEnd();
            //TestData.mTemplate = JsonConvert.DeserializeObject<Dictionary<int, TestData>>(str.ToString());
            //DataConfiguration_SO
        }
    }
}
