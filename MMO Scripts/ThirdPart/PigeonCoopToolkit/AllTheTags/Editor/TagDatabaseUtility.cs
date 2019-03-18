using PigeonCoopToolkit.AllTheTags.Internal;
using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.AllTheTags.Internal.Editor
{
    public class TagDatabaseUtility : EditorWindow
    {

        [MenuItem("Window/Pigeon Coop Toolkit/All The Tags!/Tag Manager",false,1)]
        public static void Spawn()
        {
            GetWindow<TagDatabaseUtility>(false, "Tag Manager");
        }

        private Vector2 _scrollPos;
        private static GUISkin editorSkin
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
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

        private Tag _tagBeingRenamed;
        private string _tempName;
        private string _tagToAdd;

        private TagDatabase database;
        private Object target;

        public void OnGUI()
        {
            Repaint();
            if (Application.isPlaying)
                GUI.enabled = false;

            if (database == null)
            {
                database = Resources.Load<TagDatabase>("PCTK/AllTheTags/TagDatabase");

                if (database == null)
                {
                    database = ScriptableObject.CreateInstance<TagDatabase>();
                    string path = "Assets/Plugins/Dependent/_GameLibrary/ThirdLibrary/PigeonCoopToolkit/AllTheTags/Resources/PCTK/AllTheTags/TagDatabase.asset";

                    AssetDatabase.CreateAsset(database, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    database = Resources.Load<TagDatabase>("PCTK/AllTheTags/TagDatabase");
                }

                target = database;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            for (int i = 0; i < database.Tags.Count; i++)
            {
                GUILayout.BeginHorizontal("box", GUILayout.MinHeight(30));

                Tag tag = database.Tags[i];
                if (tag == _tagBeingRenamed)
                {
                    GUI.color = tag.ReferenceColor * new Color(1, 1, 1, 0.25f);
                    GUILayout.BeginHorizontal(editorSkin.GetStyle("TagPreviewRename"), GUILayout.Width(position.width - 200));
                    GUI.color = tag.ReferenceColor;
                    _tempName = GUILayout.TextField(_tempName, editorSkin.GetStyle("TagLabel"));
                    GUILayout.EndHorizontal();
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = tag.ReferenceColor;
                    GUILayout.BeginHorizontal(editorSkin.GetStyle("TagPreview"), GUILayout.Width(position.width - 200));
                    GUILayout.Label(tag.TagFriendly, editorSkin.GetStyle("TagLabel"));
                    GUILayout.EndHorizontal();
                    GUI.color = Color.white;
                }

                GUILayout.BeginHorizontal(editorSkin.GetStyle("TagPreviewOptionsContainer"));
             
                if (_tagBeingRenamed == tag)
                {
                    if (GUILayout.Button("Save", EditorStyles.miniButton))
                    {
                        bool uniqueName = true;

                        foreach (Tag searchSameName in database.Tags)
                        {
                            if (searchSameName.TagFriendly == _tempName && searchSameName != _tagBeingRenamed)
                            {
                                uniqueName = false;
                                break;
                            }
                        }

                        if (!uniqueName)
                        {
                            EditorUtility.DisplayDialog("Error", "There is already a tag with that name, tag must be unique",
                                "Ok");
                        }
                        else
                        {
                            Undo.RecordObject(database, "Rename Tag");
                            _tagBeingRenamed.TagFriendly = _tempName;
                            EditorUtility.SetDirty(target);
                            _tagBeingRenamed = null;
                        }
                    }
                    if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                    {
                        _tagBeingRenamed = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("Rename", EditorStyles.miniButton))
                    {
                        _tagBeingRenamed = tag;
                        _tempName = tag.TagFriendly;
                    }
                    if (GUILayout.Button("Delete", EditorStyles.miniButton))
                    {
                        if (EditorUtility.DisplayDialog("Warning",
                            "You cannot undo this. Tag will be destroyed and any object with this tag will be untagged. You will have to retag everything if you wish to add this tag again in the future. Are you sure?",
                            "Yes", "No"))
                        {
                            database.Tags.RemoveAt(i);
                            i--;
                            EditorUtility.SetDirty(target);
                        }
                    }
                }


                tag.ReferenceColor = EditorGUILayout.ColorField(tag.ReferenceColor);

                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
            }

            GUILayout.EndVertical();


            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal("box");

            GUILayout.Label("Enter tag:");
            bool returnPressed = false;
            Event e = Event.current;
            if (e.keyCode == KeyCode.Return && e.rawType == EventType.KeyDown) returnPressed = true;

            GUI.SetNextControlName("TagAddField");
            _tagToAdd = EditorGUILayout.TextField(_tagToAdd);

            if (!string.IsNullOrEmpty(_tagToAdd) && returnPressed && GUI.GetNameOfFocusedControl() == "TagAddField")
            {
                database.FindOrAddTag(_tagToAdd);
                EditorUtility.SetDirty(database);
                _tagToAdd = "";
            }

            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

    }
}
