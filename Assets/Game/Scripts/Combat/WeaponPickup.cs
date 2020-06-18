using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon pickupWeapon = null;
        [SerializeField] float timeRespawn = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(pickupWeapon);
                StartCoroutine(HideForSeconds(timeRespawn));
            }
        }

        private IEnumerator HideForSeconds(float seconds) {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}
