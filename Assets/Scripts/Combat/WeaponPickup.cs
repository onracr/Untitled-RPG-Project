using Attributes;
using Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weapon = null;
        [SerializeField] private float healthToRestore = 0f;
        [SerializeField] private float respawnTime = 5f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
            subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            RevealPickup(false);
            yield return new WaitForSeconds(seconds);
            RevealPickup(true);
        }

        private void RevealPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            transform.GetChild(0).gameObject.SetActive(shouldShow);
//            foreach (Transform child in transform)
//            {
//                child.gameObject.SetActive(shouldShow);
//            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }
    }
}
