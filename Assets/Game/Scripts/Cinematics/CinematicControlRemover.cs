using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour {
        
        GameObject player;
        
        private void OnEnable() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable() {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        private void Awake() {
            player = GameObject.FindWithTag("Player");
        }
        
        void DisableControl(PlayableDirector pd){
            player.GetComponent<PlayerController>().DisablePlayer();
        }

        void EnableControl(PlayableDirector pd){
            player.GetComponent<PlayerController>().EnablePlayer();
        }
    }
}