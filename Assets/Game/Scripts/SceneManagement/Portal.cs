using System;
using System.Collections;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier{
            NONE, A,B,C,D,E,F,G
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform SpawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadingTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private Fader fader;

        private void Start() {
            fader = GameObject.FindObjectOfType<Fader>();
        }
        
        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition(){
            if(sceneToLoad < 0){
                Debug.LogError("Scene to load not set.");
                yield break;
            }
            DontDestroyOnLoad(gameObject);
            yield return fader.FadeOut(fadingTime);
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            wrapper.Load();
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            wrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadingTime);
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in GameObject.FindObjectsOfType<Portal>()){
                if(portal == this) continue;
                if(portal.destination != destination) continue;
                return portal;
            };
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if(player != null && otherPortal != null){
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.transform.position = otherPortal.SpawnPoint.position;
                player.transform.rotation = otherPortal.SpawnPoint.rotation;
                player.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}

