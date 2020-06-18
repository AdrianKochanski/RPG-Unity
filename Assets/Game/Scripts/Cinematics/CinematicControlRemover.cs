using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour {
        
        GameObject player;
        
        private void Start() {
            player = GameObject.FindWithTag("Player");
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += Enablecontrol;
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