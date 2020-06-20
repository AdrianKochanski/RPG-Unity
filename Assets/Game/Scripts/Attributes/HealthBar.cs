using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Image imageBar;

        public void DisplayHealth(float healthFraction)
        {
            if(Mathf.Approximately(healthFraction, 0) || 
            Mathf.Approximately(healthFraction, 1)) {
                gameObject.SetActive(false);
            } else {
                gameObject.SetActive(true);
                imageBar.transform.localScale = new Vector3(healthFraction, 1f, 1f);
            }
        }
    }
}
