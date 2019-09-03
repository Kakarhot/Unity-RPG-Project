using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake() 
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            Health target = fighter.GetTarget();

            if(target == null) 
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", target.GetHealth(), target.GetMaxHealthPoints());
        }
    }
}

