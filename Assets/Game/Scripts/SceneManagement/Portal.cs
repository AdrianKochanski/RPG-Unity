using System;
using System.Collections;
using GameDevTV.Utils;
using RPG.Control;
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

        private LazyValue<Fader> fader;
        GameObject player;
        PlayerController playerController;

        private void Awake() {
            fader = new LazyValue<Fader>(InitializeFader);
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        private void FindPlayerOnScene() {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        private Fader InitializeFader() {
            return GameObject.FindObjectOfType<Fader>();
        }

        private void Start() {
            fader.ForceInit();
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
            playerController.DisablePlayer();
            yield return fader.value.FadeOut(fadingTime);
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            FindPlayerOnScene();
            playerController.DisablePlayer();
            wrapper.Load();
            UpdatePlayer(GetOtherPortal());
            wrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            fader.value.FadeIn(fadingTime);
            playerController.EnablePlayer();
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

        private void UpdatePlayer(Portal otherPortal) {
            if(player != null && otherPortal != null){
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.transform.position = otherPortal.SpawnPoint.position;
                player.transform.rotation = otherPortal.SpawnPoint.rotation;
                player.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}

