using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour {
        
        GameObject player;
        
        private void OnEnable() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += Enablecontrol;
        }

        private void OnDisable() {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= Enablecontrol;
        }

        private void Awake() {
            player = GameObject.FindWithTag("Player");
        }

        private void Start() {
        }
        
        void DisableControl(PlayableDirector pd){
            player.GetComponent<ActionScheduler>().CancellCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void Enablecontrol(PlayableDirector pd){
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}