using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicTrigger : MonoBehaviour
    {
        private bool wasTriggered = false;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player") {
                if (!wasTriggered)
                {
                    MakeACutScene();
                }
            }
        }

        private void MakeACutScene()
        {
            GetComponent<PlayableDirector>().Play();
            wasTriggered = true;
        }
    }
}
