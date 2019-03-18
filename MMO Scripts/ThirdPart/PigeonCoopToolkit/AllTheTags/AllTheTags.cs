using System.Collections.Generic;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PigeonCoopToolkit.AllTheTags.Internal
{

    [AddComponentMenu("Pigeon Coop Toolkit/All The Tags!")]
    public partial class AllTheTags : MonoBehaviour
    {
        [SerializeField]
        private List<int> _tags;
        [System.NonSerialized]
        private HashSet<int> _tagHashSet;

        public bool HasTag(int tag)
        {
            if (_tags == null)
            {
                _tags = new List<int>();
            }

            if (_tagHashSet == null)
            {
                _tagHashSet = new HashSet<int>(_tags);
            }

            return _tagHashSet.Contains(tag);
        }

        void Awake()
        {
            if (_tags == null)
            {
                _tags = new List<int>();
            }

            if (_tagHashSet == null)
            {
                _tagHashSet = new HashSet<int>(_tags);
            }

            AddTagSource(gameObject, this);
        }

        void OnDestroy()
        {
            if (_tagHashSet == null)
            {
                _tagHashSet = new HashSet<int>(_tags);
            }

            RemoveTagSource(gameObject);
        }
    }

    public partial class AllTheTags
    {
        private static Dictionary<int, AllTheTags> _GOToTags;
        private static Dictionary<int, List<GameObject>> _TagsToGameObjects;

        private static TagDatabase _tagDatabase;

        /// <summary>
        /// We return this when we need to return an empty 
        /// collection, instead of allocating a new one
        /// </summary>
        private static ReadOnlyCollection<GameObject> _empty;

        private static void AddTagSource(GameObject go, AllTheTags tagSource)
        {
            if (PreExecutionChecklist() == false)
                return;

            if (!_GOToTags.ContainsKey(go.GetInstanceID()))
            {
                _GOToTags.Add(go.GetInstanceID(), tagSource);
                List<GameObject> _goCollection = null;

                for (int i = 0; i < tagSource._tags.Count; i++)
                {
                    _goCollection = null;
                    _TagsToGameObjects.TryGetValue(tagSource._tags[i], out _goCollection);

                    if (_goCollection == null)
                    {
                        _goCollection = new List<GameObject>();
                        _TagsToGameObjects[tagSource._tags[i]] = _goCollection;
                    }

                    _goCollection.Add(go);
                }
            }
        }

        private static void RemoveTagSource(GameObject go)
        {
            if (PreExecutionChecklist() == false)
                return;

            if (_GOToTags.ContainsKey(go.GetInstanceID()))
            {
                AllTheTags tagSource = _GOToTags[go.GetInstanceID()];
                _GOToTags.Remove(go.GetInstanceID());

                List<GameObject> _goCollection = null;

                for (int i = 0; i < tagSource._tags.Count; i++)
                {
                    _goCollection = null;
                    _TagsToGameObjects.TryGetValue(tagSource._tags[i], out _goCollection);

                    if (_goCollection != null && _goCollection.Contains(go))
                    {
                        _goCollection.Remove(go);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the GameObject has the tag attached. Tags are case sensitive.
        /// </summary>
        public static bool HasTag(GameObject go, string tag)
        {
            if (PreExecutionChecklist() == false)
                return false;

            AllTheTags targetTags = null;

            if (!_GOToTags.TryGetValue(go.GetInstanceID(), out targetTags))
            {
                targetTags = go.GetComponent<AllTheTags>();
            }

            if (targetTags != null)
            {
                int tagID = _tagDatabase.TagFriendlyToID(tag);

                if (tagID != -1 && targetTags.HasTag(tagID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds the tag to a GameObject. You can only add tags that have already been defined in the Tag Manager if you use this function at run time. (You can’t “define” new tags on the fly). Tags are case sensitive.
        /// </summary>
        public static void AddTag(GameObject go, string tag)
        {
            if (PreExecutionChecklist() == false)
                return;

            AllTheTags targetTags = null;
            bool isGOInScene = _GOToTags.TryGetValue(go.GetInstanceID(), out targetTags);
            if (isGOInScene == false)
            {
                targetTags = go.GetComponent<AllTheTags>();

                if (targetTags == null)
                    targetTags = go.AddComponent<AllTheTags>();
            }

            int tagID = _tagDatabase.TagFriendlyToID(tag);
            if (tagID != -1 && !targetTags.HasTag(tagID))
            {
                targetTags._tags.Add(tagID);
                targetTags._tagHashSet.Add(tagID);

                if (isGOInScene)
                {
                    List<GameObject> taggedGameObjects = null;
                    _TagsToGameObjects.TryGetValue(tagID, out taggedGameObjects);
                    if (taggedGameObjects != null && !taggedGameObjects.Contains(go))
                        taggedGameObjects.Add(go);
                }
            }
            else if (tagID == -1)
            {
                Debug.LogError("AllTheTags: You can only add tags that have already been defined in the Tag Manager to an object at run time.");
            }
        }

        /// <summary>
        /// Removes a tag from a GameObject. Tags are case sensitive.
        /// </summary>
        public static void RemoveTag(GameObject go, string tag)
        {
            if (PreExecutionChecklist() == false)
                return;


            AllTheTags targetTags = null;
            bool isGOInScene = _GOToTags.TryGetValue(go.GetInstanceID(), out targetTags);
            if (isGOInScene == false)
            {
                targetTags = go.GetComponent<AllTheTags>();

                if (targetTags == null)
                    targetTags = go.AddComponent<AllTheTags>();
            }

            int tagID = _tagDatabase.TagFriendlyToID(tag);
            if (tagID != -1 && targetTags.HasTag(tagID))
            {
                targetTags._tags.Remove(tagID);
                targetTags._tagHashSet.Remove(tagID);

                if (isGOInScene)
                {
                    List<GameObject> taggedGameObjects = null;
                    _TagsToGameObjects.TryGetValue(tagID, out taggedGameObjects);
                    if (taggedGameObjects != null)
                        taggedGameObjects.Remove(go);
                }
            }
            else if (tagID == -1)
            {
                Debug.LogError("AllTheTags: You can only remove tags that have already been defined in the Tag Manager to an object at run time.");
            }
        }

        /// <summary>
        /// Finds all GameObjects with tag tag in the current scene. Returns both active and inactive objects. Never returns null.
        /// </summary>
        public static ReadOnlyCollection<GameObject> FindGameObjectsWithTag(string tag)
        {
            if (_empty == null)
                _empty = new ReadOnlyCollection<GameObject>(new List<GameObject>());

            if (PreExecutionChecklist() == false)
                return _empty;

            int tagID = _tagDatabase.TagFriendlyToID(tag);

            if (tagID != -1)
            {
                List<GameObject> taggedList = null;
                _TagsToGameObjects.TryGetValue(tagID, out taggedList);

                if (taggedList == null)
                    taggedList = new List<GameObject>();

                return taggedList.AsReadOnly();
            }
            else
            {
                Debug.LogError("AllTheTags: You can only add tags that have already been defined in the Tag Manager to an object at run time.");
            }

            return _empty;
        }

        /// <summary>
        /// Finds all GameObjects with tag tag in the current scene within distance units away from worldPoint. Returns both active and inactive objects. Never returns null. 
        /// </summary>
        public static List<GameObject> FindGameObjectsWithTagAroundPoint(string tag, Vector3 worldPoint, float distance)
        {
            if (PreExecutionChecklist() == false)
                return new List<GameObject>();

            float distanceSquared = distance * distance;

            var withTag = FindGameObjectsWithTag(tag);
            List<GameObject> qualifyingGameObjects = new List<GameObject>();

            for (int i = 0; i < withTag.Count; i++)
            {
                if ((withTag[i].transform.position - worldPoint).sqrMagnitude < distanceSquared)
                    qualifyingGameObjects.Add(withTag[i]);
            }

            return qualifyingGameObjects;
        }

        /// <summary>
        /// Finds all children of target GameObject with a certain tag. Includes the target GameObject too. Never returns null.
        /// </summary>
        public static List<GameObject> FindChildrenWithTag(GameObject go, string tag, bool includeInactive)
        {
            if (PreExecutionChecklist() == false)
                return new List<GameObject>();

            int tagID = _tagDatabase.TagFriendlyToID(tag);

            if (tagID != -1)
            {
                return CollectChildrenWithTag(tagID, go.transform, includeInactive);
            }
            else
            {
                Debug.LogError("AllTheTags: You can only add tags that have already been defined in the Tag Manager to an object at run time.");
            }

            return new List<GameObject>();

        }

        private static bool PreExecutionChecklist()
        {
            if (!Application.isPlaying)
            {
                if (_GOToTags == null)
                    _GOToTags = new Dictionary<int, AllTheTags>();

                if (_TagsToGameObjects == null)
                    _TagsToGameObjects = new Dictionary<int, List<GameObject>>();

                if (_tagDatabase == null)
                {
                    //Assets/Standard Assets/GameFramework/ThirdLibrary/PigeonCoopToolkit/AllTheTags/Resources/PCTK/AllTheTags/TagDatabase.asset
                    
                    #if UNITY_EDITOR
                    var src = AssetDatabase.LoadAssetAtPath<TagDatabase>("Assets/Standard Assets/GameFramework/ThirdLibrary/PigeonCoopToolkit/AllTheTags/Resources/PCTK/AllTheTags/TagDatabase.asset");
                    if (src)
                    {
                        _tagDatabase = Instantiate(src) as TagDatabase;
                        _tagDatabase.Initialize();
                    }
                    else
                    {
                        return false;
                    }
                    #endif
                }
                return true;

            }
            else
            {
                if (_GOToTags == null)
                    _GOToTags = new Dictionary<int, AllTheTags>();

                if (_TagsToGameObjects == null)
                    _TagsToGameObjects = new Dictionary<int, List<GameObject>>();

                if (_tagDatabase == null)
                {
                    Object src = Resources.Load<TagDatabase>("PCTK/AllTheTags/TagDatabase");
                    if (src)
                    {
                        _tagDatabase = Instantiate(src) as TagDatabase;
                        _tagDatabase.Initialize();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static List<GameObject> CollectChildrenWithTag(int tagID, Transform activeTransform, bool includeInactive)
        {
            //Faster than recursively fetching children.. some things Unity does better!
            List<GameObject> collectedChildren = new List<GameObject>();
            var allChildren = activeTransform.GetComponentsInChildren<Transform>();

            for ( int i = 0; i < allChildren.Length; i++ )
            {
                if(!includeInactive && !allChildren[i].gameObject.activeInHierarchy)
                    continue;

                AllTheTags att = null;
                if (_GOToTags.TryGetValue(allChildren[i].gameObject.GetInstanceID(), out att))
                {
                    if (att.HasTag(tagID))
                    {
                        collectedChildren.Add(allChildren[i].gameObject);
                    }
                }
            }

            return collectedChildren;
        }
        
    }

}

namespace PigeonCoopToolkit.AllTheTags
{

    /// <summary>
    /// This static class provides a nice clean way to access AllTheTags! methods
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// Checks if the GameObject has the tag attached. Tags are case sensitive.
        /// </summary>
        public static bool HasTag(GameObject go, string tag)
        {
            return Internal.AllTheTags.HasTag(go, tag);
        }

        /// <summary>
        /// Adds the tag to a GameObject. You can only add tags that have already been defined in the Tag Manager if you use this function at run time. (You can’t “define” new tags on the fly). Tags are case sensitive.
        /// </summary>
        public static void AddTag(GameObject go, string tag)
        {
            Internal.AllTheTags.AddTag(go, tag);
        }

        /// <summary>
        /// Removes a tag from a GameObject. Tags are case sensitive.
        /// </summary>
        public static void RemoveTag(GameObject go, string tag)
        {
            Internal.AllTheTags.RemoveTag(go, tag);
        }

        /// <summary>
        /// Finds all GameObjects with tag tag in the current scene. Returns both active and inactive objects. Never returns null.
        /// </summary>
        public static ReadOnlyCollection<GameObject> FindGameObjectsWithTag(string tag)
        {
            return Internal.AllTheTags.FindGameObjectsWithTag(tag);
        }

        /// <summary>
        /// Finds all GameObjects with tag tag in the current scene within distance units away from worldPoint. Returns both active and inactive objects. Never returns null. 
        /// </summary>
        public static List<GameObject> FindGameObjectsWithTagAroundPoint(string tag, Vector3 worldPoint, float distance)
        {
            return Internal.AllTheTags.FindGameObjectsWithTagAroundPoint(tag, worldPoint, distance);
        }

        /// <summary>
        /// Finds all children of target GameObject with a certain tag. Includes the target GameObject too. Returns both active and inactive objects. Never returns null.
        /// </summary>
        public static List<GameObject> FindChildrenWithTag(GameObject go, string tag)
        {
            return Internal.AllTheTags.FindChildrenWithTag(go, tag, true);
        }

        /// <summary>
        /// Finds all children of target GameObject with a certain tag. Includes the target GameObject too. Never returns null.
        /// </summary>
        public static List<GameObject> FindChildrenWithTag(GameObject go, string tag, bool includeInactive)
        {
            return Internal.AllTheTags.FindChildrenWithTag(go, tag, includeInactive);
        }

    }

}
