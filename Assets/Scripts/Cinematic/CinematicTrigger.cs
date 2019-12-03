using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematic
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _isActivated = false;
        private void OnTriggerEnter(Collider other)
        {
            if (!_isActivated && other.CompareTag("Player"))
            {
                GetComponent<PlayableDirector>().Play();
                _isActivated = true;
            }
        }
    }

}