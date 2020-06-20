using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        //Text display;
        // Start is called before the first frame update
        private void Awake()
        {
            //display = gameObject.GetComponent<Text>();
        }

        // Update is called once per frame
        private void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
