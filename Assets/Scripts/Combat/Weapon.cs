﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;
        
        public void OnHit()
        {
            onHit?.Invoke();
        }
    }
}
