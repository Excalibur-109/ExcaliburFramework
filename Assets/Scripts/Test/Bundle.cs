using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ABDependsAnalyze
{
    private class ABNode
    {
        public string _name;
        public string _path;
        public bool _isRoot; //�Ƿ���root�ڵ�

        public bool _isCombine; //�Ƿ���������ͬ���ϲ���һ��ab
        public List<string> _combineList = new List<string>();

        public Dictionary<string, ABNode> _depends = new Dictionary<string, ABNode>();
        public Dictionary<string, ABNode> _beDepends = new Dictionary<string, ABNode>();
    }

    private static ABDependsAnalyze _Instance = null;
    public static ABDependsAnalyze I
    {
        get
        {
            if (_Instance == null) _Instance = new ABDependsAnalyze();
            return _Instance;
        }
    }

    private Dictionary<string, ABNode> _ABNodeList; //<path, node>
    private List<AssetBundleBuild> _ABBuildList;

    private ABDependsAnalyze()
    {
        _ABNodeList = new Dictionary<string, ABNode>();
        _ABBuildList = new List<AssetBundleBuild>();
    }

    private void Clear()
    {
        _ABNodeList.Clear();
        _ABBuildList.Clear();
    }

    //���ﴫ���·�������ȫ·��, ������Ҫ������ļ��б�
    //Ȼ�����BuildPipeline.BuildAssetBundles(exportPath, AssetBundleBuild[], xxx,xxx);�Ϳ�����
    public AssetBundleBuild[] Analyze(List<string> dirPaths, List<string> filePaths)
    {
        List<string> targetFiles = new List<string>();

        foreach (string dirPath in dirPaths)
        {
            string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                targetFiles.Add(file);
            }
        }

        targetFiles.AddRange(filePaths);

        AnalyzeComplex(targetFiles);

        return _ABBuildList.ToArray();
    }

    private bool IsFileVaild(string filePath)
    {//���߿����Լ�ʵ���ļ���Ч��
        filePath = filePath.ToLower();

        if (filePath.EndsWith(".meta")) return false;
        if (filePath.EndsWith(".cs")) return false;
        if (filePath.EndsWith(".dll")) return false;

        return true;
    }

    private string GetAbName(ABNode abNode)
    {//���߿����Լ�ʵ��·��ת����ʽ������ṩһ�ַ���
        if (abNode._isRoot)
        {//root�ڵ�������·��Ϊ����·��
            return abNode._name.Replace("Assets/", "") + ".ab";
        }
        else
        {//�����ڵ���guid��Ϊ·��
            return "depends/" + AssetDatabase.AssetPathToGUID(abNode._path) + ".ab";
        }
    }

    private void AnalyzeComplex(List<string> files)
    {
        Clear();

        //���������ڵ㣬����һ�������޻�ͼ
        for (int i = 0; i < files.Count; ++i)
        {
            //��ȡ��unity��ԴAssetsĿ¼��·��
            string unityPath = files[i].Substring(files[i].IndexOf("Assets/"));
            unityPath = unityPath.Replace("\\", "/");

            if (!IsFileVaild(unityPath)) continue;

            var abNode = CreateNode(unityPath, true);
            if (!_ABNodeList.ContainsKey(unityPath))
            {
                _ABNodeList.Add(unityPath, abNode);
                AnalyzeNode(abNode);
            }
            else abNode._isRoot = true;
        }

        //��֦��ȥ�ߣ�����������ϸ������ɭ�֣�
        //�������ȥ��������A->B,B->C,A->C, ȥ��A->C������
        bool flag = true;
        while (flag)
        {
            flag = false;
            foreach (var abNode in _ABNodeList.Values)
            {
                if (abNode._isRoot) continue;
                var depends = abNode._depends.Values.ToArray();
                foreach (var dependNode in depends)
                {
                    var beDepends = abNode._beDepends.Values.ToArray();
                    foreach (var beDependNode in beDepends)
                    {
                        flag |= DeleteRepeatNode(beDependNode, dependNode);
                    }
                }
            }
        }

        //���㣬������������
        //�������ȥ��������A->B,B->C, ���BC��û�б������ڵ�������BC�Ͳ���Ҫ������
        flag = true;
        while (flag)
        {
            flag = false;
            var abNodes = _ABNodeList.Values.ToArray();
            foreach (var abNode in abNodes)
            {
                if (abNode._isRoot) continue;
                if (abNode._beDepends.Count != 1) continue;

                var beDepend = abNode._beDepends.Values.ToArray()[0];
                beDepend._depends.Remove(abNode._path);
                foreach (var depend in abNode._depends.Values)
                {
                    depend._beDepends.Remove(abNode._path);
                    if (!depend._beDepends.ContainsKey(beDepend._path))
                        depend._beDepends.Add(beDepend._path, beDepend);
                    if (!beDepend._depends.ContainsKey(depend._path))
                        beDepend._depends.Add(depend._path, depend);
                }

                _ABNodeList.Remove(abNode._path);
                flag = true;
            }
        }

        //�ϵ㣬����������
        //��ͬ�����ϲ���1���ڵ㣬����A->B,A->C,D->B,D->C,��ôBC���Դ�һ��ab���棬����ab����
        flag = true;
        while (flag)
        {
            flag = false;
            var abNodes = _ABNodeList.Values.OrderBy(a => a._path).ToArray();
            for (int i = 0; i < abNodes.Length; i++)
            {
                if (abNodes[i]._isRoot) continue;
                if (abNodes[i]._isCombine) continue;
                if (abNodes[i]._path.ToLower().EndsWith(".shader")) continue;

                for (int j = i + 1; j < abNodes.Length; j++)
                {
                    if (abNodes[j]._isRoot) continue;
                    if (abNodes[j]._isCombine) continue;
                    if (abNodes[j]._path.ToLower().EndsWith(".shader")) continue;

                    if (!IsBeDependsEqual(abNodes[i], abNodes[j])) continue;

                    abNodes[i]._combineList.Add(abNodes[j]._path);
                    abNodes[j]._isCombine = true;
                    flag = true;
                }
            }
        }


        foreach (var abNode in _ABNodeList.Values)
        {
            if (abNode._isCombine) continue;

            AssetBundleBuild abBuild = new AssetBundleBuild();
            abBuild.assetBundleName = GetAbName(abNode);
            abBuild.assetNames = abNode._combineList.ToArray();
            _ABBuildList.Add(abBuild);
        }
    }

    private bool IsBeDependsEqual(ABNode a, ABNode b)
    {
        if (a._beDepends.Count != b._beDepends.Count) return false;
        if (a._beDepends.Count == 0) return false;

        foreach (var beDepend in a._beDepends.Values)
        {
            if (!b._beDepends.ContainsKey(beDepend._path)) return false;
        }

        return true;
    }

    private ABNode CreateNode(string filePath, bool isRoot)
    {
        ABNode abNode = null;
        if (_ABNodeList.ContainsKey(filePath))
        {
            abNode = _ABNodeList[filePath];
            return abNode;
        }

        abNode = new ABNode();
        abNode._name = Path.GetFileNameWithoutExtension(filePath);
        abNode._path = filePath;
        abNode._isRoot = isRoot;
        abNode._combineList.Add(filePath);

        return abNode;
    }

    private void AnalyzeNode(ABNode abNode)
    {
        string[] depend_paths = AssetDatabase.GetDependencies(abNode._path);
        foreach (var tempDependPath in depend_paths)
        {
            string dependPath = tempDependPath.Replace("\\", "/");
            if (dependPath == abNode._path) continue;

            if (!IsFileVaild(dependPath)) continue;

            ABNode abDependNode = CreateNode(dependPath, false);
            abNode._depends.Add(dependPath, abDependNode);
            abDependNode._beDepends.Add(abNode._path, abNode);

            if (!_ABNodeList.ContainsKey(dependPath))
            {
                _ABNodeList.Add(dependPath, abDependNode);
                AnalyzeNode(abDependNode);
            }

        }

    }

    private bool DeleteRepeatNode(ABNode abNode, ABNode deleteNode)
    {
        bool flag = false;
        if (abNode._depends.ContainsKey(deleteNode._path))
        {
            abNode._depends.Remove(deleteNode._path);
            deleteNode._beDepends.Remove(abNode._path);

            //Repo.Log(ELogType.Load, "delete " + abNode._path + "\n" + deleteNode._path);
            flag = true;
        }

        var beDepends = abNode._beDepends.Values.ToArray();
        foreach (var beDependNode in beDepends)
        {
            flag |= DeleteRepeatNode(beDependNode, deleteNode);
        }

        return flag;
    }
}

