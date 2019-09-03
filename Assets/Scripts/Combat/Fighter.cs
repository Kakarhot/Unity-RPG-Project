using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    { 
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Transform rightHand = null;

        Health target;
        Mover moveComponent;
        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<WeaponConfig> currentWeapon;
        Weapon currentWeaponPrefab;

        public WeaponConfig GetCurrentWeapon() { return currentWeapon.value; }
        public Health GetTarget() { return target; }

        private void Awake() {
            moveComponent = GetComponent<Mover>();
            currentWeapon = new LazyValue<WeaponConfig>(SetupDefaultWeapon);
        }

        private WeaponConfig SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update() 
        {
            timeSinceLastAttack += Time.deltaTime;

            if(target == null || target.IsDead()) return;
            if(!currentWeapon.value) return;

            if(Vector3.Distance(transform.position, target.transform.position) > currentWeapon.value.GetWeaponRange())
            {
                moveComponent.MoveTo(target.transform.position);
            }
            else
            {
                moveComponent.Cancel();
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    AttackBehaviour();
                    timeSinceLastAttack = 0;
                }              
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            AttachWeapon(weapon);
            currentWeapon.value = weapon;
        }

        void AttachWeapon(WeaponConfig weapon)
        {
            if (!weapon || !leftHand || !rightHand) return;

            currentWeaponPrefab = weapon.SpawnWeapon(leftHand, rightHand, GetComponent<Animator>());
        }
        
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            GetComponent<Animator>().ResetTrigger("StopAttack");
            // This will trigger the Hit() event.
            GetComponent<Animator>().SetTrigger("Attack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            Health healthToTest = GetComponent<Health>();
            return healthToTest!=null && !healthToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionSchedule>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
            target = null;
            GetComponent<Mover>().Cancel();
        }

        // Animation Event
        void Hit()
        {
            if(!target) return;
            target.TakeDamage(gameObject, GetComponent<BaseStats>().GetStat(Stat.Damage));
            if(currentWeaponPrefab)  
            {
                currentWeaponPrefab.onHit.Invoke();
            }
        }

        // Animation Event
        void Shoot()
        {
            if(!target) return;
            currentWeapon.value.SpawnProjectile(target.transform, gameObject, leftHand, rightHand, GetComponent<BaseStats>().GetStat(Stat.Damage));
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageDamage();
            }
        }
        
        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string)state);
            EquipWeapon(weapon);
        }
    }
}
