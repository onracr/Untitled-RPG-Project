﻿using System;
using System.Collections;
using System.Collections.Generic;
using Control;
using Core;
using Attributes;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed = 1f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private float maxLifeTime = 5f;
        [SerializeField] private float lifeAfterImpact = 2f;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private UnityEvent onHit;


        private GameObject _instigator = null;
        private Health _target = null;
        private float _damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) return;
            
            if (isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            
            transform.Translate(Time.deltaTime * projectileSpeed * Vector3.forward);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }
        
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            
            if (targetCapsule == null)
                return _target.transform.position;
            
            return _target.transform.position + targetCapsule.height / 2 * Vector3.up;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead()) return;
            _target.TakeDamage(_instigator, _damage);

            projectileSpeed = 0f;
            onHit?.Invoke();
            
            if (hitEffect)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (var toDestroy in destroyOnHit)
            {
                  Destroy(toDestroy);  
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }

}