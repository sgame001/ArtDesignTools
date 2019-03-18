using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HKLibrary;
using UnityEditor;

public class HKResourcesMappingEditor : Editor
{
    [MenuItem("HKTools/ResTools/Mapping Resources")]
    public static void MappingResources()
    {
        MapResources(false);
    }
    
    public static void MapResources(bool _isShowDialog = true)
    {
        UnityEngine.Debug.Log("xxxxxxxxxxxxx");
        // resources的路径
        string RESOURCES_PATH = Application.dataPath + "/ClientResources";

        // 配置文件的路径
        const string MAPCONFIG_PATH = "Assets/Resources/Config/ResourcesMap.json";

        // 有效文件，排除 meta
        List<FileInfo> ValidFileInfos = new List<FileInfo>();


        List<FileInfo> allResourcesFiles = new List<FileInfo>();


//        var paths = Selection.instanceIDs;
//        for (int index = 0; index < paths.Length; index++)
//        { 资源加载失败 
//            var assetGuid = paths[index];
//            var path = AssetDatabase.GetAssetPath(assetGuid);
//
//            
//        }
        
        // 设置根目录，并且获取所有的资源
        DirectoryInfo dirInfo = new DirectoryInfo(RESOURCES_PATH);
        HKEditorFileHelper.GetAllFileBySub(dirInfo, allResourcesFiles, true);

        if (allResourcesFiles.Count > 0)
        {
            for (int index = 0; index < allResourcesFiles.Count; index++)
            {
                FileInfo fi = allResourcesFiles[index];
                if (null == fi)
                {
                    continue;
                }

                if (fi.Extension == ".meta")
                {
                    continue;
                }

                if (fi.Name == "ResourcesMap.json")
                {
                    continue;
                }

                ValidFileInfos.Add(fi);
            }
        }


        Dictionary<string, HKResourcesMapField> mapFields = new Dictionary<string, HKResourcesMapField>();

        // 失败路径，有可能是路径无效，也有可能是文件对象为空
        List<string> failList = new List<string>();

        // 遍历文件路径
        for (int index = 0; index < ValidFileInfos.Count; index++)
        {
            FileInfo fileInfo = ValidFileInfos[index];

            if (null == fileInfo)
            {
                continue;
            }

            // 获取文件名字
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            // 获取完整路径
            string fullPath = ValidFileInfos[index].FullName;
            fullPath = fullPath.Replace("\\", "/");

            // Resources的相对路径
            string resPath = HKEditorFileHelper.GetRelativePathByName(fullPath, "Assets");

            if (true == string.IsNullOrEmpty(fileName) || true == string.IsNullOrEmpty(resPath))
            {
                continue;
            }

            string fileExtension = Path.GetExtension(resPath);
            string resourcesPath = resPath.Replace(fileExtension, "");
            resourcesPath = resourcesPath.Replace("Assets/Resources/", "");

            if (false == mapFields.ContainsKey(fileName))
            {
                mapFields.Add(fileName, new HKResourcesMapField()
                {
                    fileName = fileName,
                    filePath = resPath,
                    ResourcesPath = resourcesPath
                });
            }
            else
            {
                failList.Add("文件重复 = " + fileName);
            }
        }

        // 检测失败数量
        if (failList.Count > 0)
        {
            Debug.LogError("映射失败，存在同名文件");
            for (int index = 0; index < failList.Count; index++)
            {
                string failName = failList[index];
                Debug.LogError("fail name = " + failName);
            }
            return;
        }
        else
        {
            // 写入到Json文件中
            List<HKResourcesMapField> mapList = new List<HKResourcesMapField>();
            foreach (var mapField in mapFields)
            {
                mapList.Add(mapField.Value);
            }
            string mapJson = JsonUtility.ToJson(new HKSerialization<HKResourcesMapField>(mapList), true);

            // 配置文件路径
            HKEditorFileHelper.WriteTextToFile(mapJson, MAPCONFIG_PATH);

            Debug.Log("转换完成：有效文件数量 = " + ValidFileInfos.Count);
            if (true == _isShowDialog)
            {
                EditorUtility.DisplayDialog("Info", "Mapping Complete, Files Count = " + ValidFileInfos.Count,
                    "  OK  ");
            }
            else
            {
                UnityEngine.Debug.Log("<color=#00FF00>Success, Mapping Complete, Files Count = " + ValidFileInfos.Count + "</color>");
            }
            AssetDatabase.Refresh();
        }
    }
}