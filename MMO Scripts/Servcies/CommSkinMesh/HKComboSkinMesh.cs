// // ================================================================
// // FileName:HKComboSkinMesh.cs
// // User: Baron
// // CreateTime:11/17/2017
// // Description: 换装的核心方法
// // ================================================================

using System.Collections.Generic;
using UnityEngine;

namespace HKLibrary
{
    public class HKComboSkinMesh : ISkinMeshCombo
    {
        /// <summary>
        /// 合并模型
        /// </summary>
        /// <param name="_skeleton"></param>
        /// <param name="meshes"></param>
        public void Combo(GameObject _skeleton, SkinnedMeshRenderer[] meshes)
        {
            if (null == _skeleton || meshes.Length <= 0)
            {
                return;
            }
            // Fetch all bones of the skeleton
            List<Transform> transforms = new List<Transform>();
            transforms.AddRange(_skeleton.GetComponentsInChildren<Transform>(true));

            List<Material> materials = new List<Material>(); //the list of materials
            List<CombineInstance> combineInstances = new List<CombineInstance>(); //the list of meshes
            List<Transform> bones = new List<Transform>(); //the list of bones

            // Collect information from meshes
            for (int i = 0; i < meshes.Length; i++)
            {
                SkinnedMeshRenderer smr = meshes[i];
                materials.AddRange(smr.materials); // Collect materials

                // Collect meshes
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = sub;
                    combineInstances.Add(ci);
                }
//                UnityEngine.Debug.Log("smr name = " + smr.name);
                // Collect bones
                for (int j = 0; j < smr.bones.Length; j++)
                {
                    int tBase = 0;
                    for (tBase = 0; tBase < transforms.Count; tBase++)
                    {
                        if (smr.bones[j].name.Equals(transforms[tBase].name))
                        {
                            bones.Add(transforms[tBase]);
                            break;
                        }
                    }
                }
            }

            // Create a new SkinnedMeshRenderer
            SkinnedMeshRenderer oldSKinned = _skeleton.GetComponent<SkinnedMeshRenderer>();
            if (oldSKinned != null)
            {
                Object.DestroyImmediate(oldSKinned);
            }
            SkinnedMeshRenderer r = _skeleton.AddComponent<SkinnedMeshRenderer>();
            r.sharedMesh = new Mesh();
            r.receiveShadows = false; // 不接收阴影
            r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false /**不合并贴图*/, false); // Combine meshes
            r.bones = bones.ToArray(); // Use new bones
            r.materials = materials.ToArray();
        }
    }
}