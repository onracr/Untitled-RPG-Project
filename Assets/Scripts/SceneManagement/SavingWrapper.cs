using System.Collections;
using Saving;
using UnityEngine;

namespace SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = .2f;
        private const string DefaultSaveFile = "save";
        private SavingSystem _savingSystem;

        private IEnumerator Start()
        {
            _savingSystem = GetComponent<SavingSystem>();
            
            Fader fader = FindObjectOfType<Fader>();
            
            fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastScene(DefaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Save();

            if (Input.GetKeyDown(KeyCode.L))
                Load();
            if (Input.GetKeyDown(KeyCode.Delete))
                Delete();
        }
        
        public void Save()
        {
            _savingSystem.Save(DefaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(DefaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(DefaultSaveFile);
        }
    }

}