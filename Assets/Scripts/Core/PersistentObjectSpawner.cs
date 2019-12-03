using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjPrefab = null;

        private static bool _hasSpawned;
        
        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObjects();

            _hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            var persistentObj = Instantiate(persistentObjPrefab);
            DontDestroyOnLoad(persistentObj);
        }
    }
}
