using System.Collections;
using Saving;
using UnityEngine;

namespace SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = .2f;
        private const string DefaultSaveFile = "save";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile);
            
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

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
            GetComponent<SavingSystem>().Save(DefaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DefaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(DefaultSaveFile);
        }
    }

}