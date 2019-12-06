using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                int buildIndex = (int) state["lastSceneBuildIndex"];

                if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                    yield return SceneManager.LoadSceneAsync(buildIndex);
            }

            RestoreState(state);
        }
        
        public void Save(string saveFile)
        {
            var state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        private void SaveFile(string saveFile, object captureState)
        {
            var path = GetPathFromSaveFile(saveFile);
            using (var stream = File.Open(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, captureState);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            var path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            
            using (var stream = File.Open(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }
        
        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (var saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                state[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }
      
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                var id = saveableEntity.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveableEntity.RestoreState(state[id]);
                }
            }
        }
        
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}
