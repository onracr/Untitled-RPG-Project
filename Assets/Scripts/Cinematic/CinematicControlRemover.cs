﻿using Control;
using Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematic
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject _player;
        
        private void Start()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            _player = GameObject.FindWithTag("Player");
        }

        private void DisableControl(PlayableDirector obj)
        {
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl(PlayableDirector obj)
        {
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }

}
