using System.Collections.Generic;
using UnityEngine;

namespace PigeonCoopToolkit.AllTheTags.Internal
{
    public class TagDatabase : ScriptableObject
    {
        public List<Tag> Tags = new List<Tag>();

        private Dictionary<int, int> _friendlyToID;
        public void Initialize()
        {
            _friendlyToID = new Dictionary<int, int>();
            for (int i = 0; i < Tags.Count; i++)
            {
                _friendlyToID.Add(Tags[i].TagFriendly.GetHashCode(), Tags[i].TagID);
            }
        }

        public int TagFriendlyToID(string friendly)
        {
            int result;
            if ( _friendlyToID.TryGetValue(friendly.GetHashCode(), out result) )
                return result;
            return -1;
        }

        public Tag GetTagViaID( int id )
        {
            foreach ( var tag in Tags )
            {
                if ( tag.TagID == id )
                    return tag;
            }

            return null;
        }

        public Tag FindOrAddTag(string friendly)
        {
            foreach (var tags in Tags)
            {
                if ( tags.TagFriendly == friendly)
                {
                    return tags;
                }
            }

            int randomID = 0;
            bool duplicate = false;
            do
            {
                randomID = Random.Range(0, int.MaxValue);
                duplicate = false;

                foreach ( var tag in Tags )
                {
                    if ( tag.TagID == randomID )
                    {
                        duplicate = true;
                        break;
                    }
                }
            
            } while ( duplicate );

            Tag newTag = new Tag {TagFriendly = friendly, TagID = randomID, ReferenceColor = Color.white};
            Tags.Add(newTag);
            return newTag;
        }
    }

    [System.Serializable]
    public class Tag
    {
        public string TagFriendly;
        public int TagID;
        public Color ReferenceColor;
    }
}