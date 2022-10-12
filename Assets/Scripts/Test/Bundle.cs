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
        public bool _isRoot; //是否是root节点

        public bool _isCombine; //是否被依赖项相同，合并成一个ab
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

    //这里传入的路径最好是全路径, 返回需要打包的文件列表
    //然后调用BuildPipeline.BuildAssetBundles(exportPath, AssetBundleBuild[], xxx,xxx);就可以了
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
    {//读者可以自己实现文件有效性
        filePath = filePath.ToLower();

        if (filePath.EndsWith(".meta")) return false;
        if (filePath.EndsWith(".cs")) return false;
        if (filePath.EndsWith(".dll")) return false;

        return true;
    }

    private string GetAbName(ABNode abNode)
    {//读者可以自己实现路径转换方式，这边提供一种方法
        if (abNode._isRoot)
        {//root节点以所在路径为加载路径
            return abNode._name.Replace("Assets/", "") + ".ab";
        }
        else
        {//依赖节点以guid作为路径
            return "depends/" + AssetDatabase.AssetPathToGUID(abNode._path) + ".ab";
        }
    }

    private void AnalyzeComplex(List<string> files)
    {
        Clear();

        //建立依赖节点，生成一张有向无环图
        for (int i = 0; i < files.Count; ++i)
        {
            //提取在unity资源Assets目录下路径
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

        //剪枝（去边），变成树（严格意义叫森林）
        //将多余边去掉，比如A->B,B->C,A->C, 去掉A->C这条边
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

        //减点，变成最简依赖树
        //将多余点去掉，比如A->B,B->C, 如果BC都没有被其他节点依赖，BC就不需要在树中
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

        //合点，减少总数量
        //相同依赖合并成1个节点，比如A->B,A->C,D->B,D->C,那么BC可以打到一个ab里面，减少ab数量
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

