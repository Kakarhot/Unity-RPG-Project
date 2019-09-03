using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        // A class has generics in it can't be used as serializeField
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        LazyValue<float> healthPoints;
        bool isDead = false;

        public bool IsDead() { return isDead; }
        public float GetHealth() { return healthPoints.value; }

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable() {
            GetComponent<BaseStats>().onLevelUp += RegenHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().onLevelUp -= RegenHealth;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value-damage, 0);

            if(healthPoints.value<=0 && !isDead)
            {
                Die();
                AwardExperience(instigator);
                onDie.Invoke();
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(!experience) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.Experience));
        }

        public float GetPercentage()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health)) * 100;
        }

        void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionSchedule>().CancelCurrentAction();

            if (!GetComponent<CapsuleCollider>()) return;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        void RegenHealth()
        {
            healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            float health = (float)state;
            healthPoints.value = health;
            if (healthPoints.value <= 0 && !isDead)
            {
                Die();
            }
        }
    }
}
