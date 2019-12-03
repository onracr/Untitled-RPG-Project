using System;
using System.Collections;
using Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        { A, B, C, D }
        
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint = null;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutTime = .5f;
        [SerializeField] private float fadeInTime = 1f;
        [SerializeField] private float fadeWaitTime = .5f;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                StartCoroutine(SceneTransition());
        }

        private IEnumerator SceneTransition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set!");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            var fader = FindObjectOfType<Fader>();
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            
            yield return fader.FadeOut(fadeOutTime);
            
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            var player = GameObject.FindWithTag("Player");
            // player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                
                return portal;
            }

            return null;
        }
    }
}
