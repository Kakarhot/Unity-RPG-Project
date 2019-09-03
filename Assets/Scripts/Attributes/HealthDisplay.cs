using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake() 
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealthPoints());
        }
    }
}

