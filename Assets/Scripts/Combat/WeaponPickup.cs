using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon = null;
        [SerializeField] private float respawnTime = 5f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
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
    }
}
