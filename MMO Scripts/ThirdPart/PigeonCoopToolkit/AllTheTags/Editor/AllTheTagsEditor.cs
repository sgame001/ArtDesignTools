using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.AllTheTags.Internal.Editor
{
    [CustomEditor(typeof(AllTheTags))]
    public class AllTheTagsEditor : UnityEditor.Editor
    {
        private static TagDatabase tagDatabase;

        private static GUISkin editorSkin
        {
            get
            {
                if ( EditorGUIUtility.isProSkin )
                {
                    if (editorSkinPro == null)
                    {
                        editorSkinPro = Resources.Load<GUISkin>("PCTK/AllTheTags/AllTheTagsSkin");
                    }

                    return editorSkinPro;
                }
                else
                {
                    if (editorSkinLight == null)
                    {
                        editorSkinLight = Resources.Load<GUISkin>("PCTK/AllTheTags/AllTheTagsSkinLight");
                    }

                    return editorSkinLight;
                }
            }
        }
        private static GUISkin editorSkinPro;
        private static GUISkin editorSkinLight;

        private string _tagToAdd;

        void OnEnable()
        {
            foreach (Object o in targets)
            {
                AllTheTags activeATT = o as AllTheTags;
                if (activeATT != null)
                {
                    var allthetags = activeATT.GetComponents<AllTheTags>();
                    if ( allthetags.Length <= 1 )
                        continue;

                    for ( int i = 1; i < allthetags.Length; i++ )
                    {
                        DestroyImmediate(allthetags[i]);
                    }

                    EditorUtility.DisplayDialog("Error",
                        "You can only have one 'AllTheTags' component attached to your GameObject", "OK");
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                Repaint();

            GUILayout.Space(10);


            AllTheTags att = (AllTheTags)target;

            if ( tagDatabase == null )
            {
                tagDatabase = Resources.Load<TagDatabase>("PCTK/AllTheTags/TagDatabase");

                if ( tagDatabase == null )
                {
                    tagDatabase = ScriptableObject.CreateInstance<TagDatabase>();
                    string path = "Assets/PigeonCoopToolkit/AllTheTags/Resources/PCTK/AllTheTags/TagDatabase.asset";

                    AssetDatabase.CreateAsset(tagDatabase, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    tagDatabase = Resources.Load<TagDatabase>("PCTK/AllTheTags/TagDatabase");
                }
            }

        
            List<int> activeTagList =
                typeof(AllTheTags).GetField("_tags", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(att) as List<int>;
            bool activeTagListModified = false;

            if ( activeTagList == null )
            {
                activeTagListModified = true;
                activeTagList = new List<int>();
            }


            float currentLabelPos = 0;

            GUILayout.BeginHorizontal();

            for (int i = 0; i < activeTagList.Count; i++)
            {
                Tag tag = tagDatabase.GetTagViaID(activeTagList[i]);
                bool removeFromList = false;

                if (tag == null)
                {
                    removeFromList = true;
                }
                else
                {
                    float minWidth, maxWidth;
                    editorSkin.GetStyle("Tag").CalcMinMaxWidth(new GUIContent(tag.TagFriendly), out minWidth, out maxWidth);
                    currentLabelPos += (maxWidth + 20);
                    if (currentLabelPos > Screen.width)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        currentLabelPos = maxWidth;
                    }

                    GUI.color = tag.ReferenceColor;
                    GUILayout.Label(new GUIContent(tag.TagFriendly), editorSkin.GetStyle("Tag"));
                    Rect labelRect = GUILayoutUtility.GetLastRect();

                    labelRect = new Rect(labelRect.xMax - 18, labelRect.yMin + 4, 14, 14);
                    if (GUI.Button(labelRect, "", editorSkin.GetStyle("Remove")))
                    {
                        removeFromList = true;
                    }
                }

                if (removeFromList)
                {
                    Undo.RecordObject(att, "Remove Tag");
                    activeTagList.RemoveAt(i);
                    activeTagListModified = true;
                    i--;
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter tag:");

            bool returnPressed = false, tabPressed = false;
            Event e = Event.current;
            if (e.keyCode == KeyCode.Return && e.rawType == EventType.KeyDown) returnPressed = true;
            if (e.keyCode == KeyCode.Tab && e.rawType == EventType.KeyDown) tabPressed = true;
            GUI.SetNextControlName("TagAddField");
            _tagToAdd = EditorGUILayout.TextField(_tagToAdd);
            Rect suggestionRect = GUILayoutUtility.GetLastRect();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Manage Tags", EditorStyles.miniButton))
            {
                TagDatabaseUtility.Spawn();
                //Selection.activeObject = tagDatabase;
            }
            GUILayout.EndVertical();


            if (!string.IsNullOrEmpty(_tagToAdd) && returnPressed && GUI.GetNameOfFocusedControl() == "TagAddField")
            {
                Tag newTag = tagDatabase.FindOrAddTag(_tagToAdd);
                EditorUtility.SetDirty(tagDatabase);

                if (!activeTagList.Contains(newTag.TagID))
                {
                    Undo.RecordObject(att, "Attach Tag");
                    activeTagList.Add(newTag.TagID);
                    activeTagListModified = true;
                    _tagToAdd = "";
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "This object has already been tagged as " + _tagToAdd + ".", "Ok");
                }
            }

            if ( !string.IsNullOrEmpty(_tagToAdd) )
            {
                Tag suggestedTag = null;
            
                foreach (var tag in tagDatabase.Tags)
                {
                    if (tag.TagFriendly.Length > _tagToAdd.Length && tag.TagFriendly.StartsWith(_tagToAdd) && !activeTagList.Contains(tag.TagID) )
                    {
                        if ( suggestedTag != null && suggestedTag.TagFriendly.Length < tag.TagFriendly.Length )
                            continue;

                        suggestedTag = tag;
                    }
                }

                if (suggestedTag != null)
                {
                    suggestionRect.xMin += 1;
                    suggestionRect.xMax += 1;
                    Color restoreColor = GUI.color;
                    GUI.color = restoreColor * new Color(1, 1, 1, 0.25f);
                    GUI.Label(suggestionRect, suggestedTag.TagFriendly);
                    GUI.color = restoreColor;

                    if ( tabPressed )
                    {
                        e.Use();

                        Undo.RecordObject(att, "Attach Tag");
                        activeTagList.Add(suggestedTag.TagID);
                        activeTagListModified = true;
                        _tagToAdd = "";
                    }
                }
            }

            if (activeTagListModified)
            {
                typeof(AllTheTags).GetField("_tags", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(att, activeTagList);

                EditorUtility.SetDirty(att);

                Repaint();
            }
        }

    }
}

