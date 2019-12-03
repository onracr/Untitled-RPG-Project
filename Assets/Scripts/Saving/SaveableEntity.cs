using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saving
{
    [ExecuteAlways]    
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = System.Guid.NewGuid().ToString();
        private static Dictionary<string, SaveableEntity> _globalLookup = new Dictionary<string, SaveableEntity>();
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();
            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>) state;
            print(stateDict.Count);
            foreach (var saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                    saveable.RestoreState(stateDict[typeString]);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;
                        
            var serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(serializedProperty.stringValue) || !IsUnique(serializedProperty.stringValue))
            {
                serializedProperty.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            _globalLookup[serializedProperty.stringValue] = this;
        }
#endif
        
        private bool IsUnique(string candidate)
        {
            if (!_globalLookup.ContainsKey(candidate)) return true;

            if (_globalLookup[candidate] == this) return true;

            if (_globalLookup[candidate] == null)
            {
                _globalLookup.Remove(candidate); 
                return true;
            }

            if (_globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                _globalLookup.Remove(candidate);
                return true;
            }
            
            return false;
        }
    }
}