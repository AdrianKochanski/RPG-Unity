using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool autoGuidance = false;
        [SerializeField] GameObject impactEffect = null;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 1f;
        [SerializeField] UnityEvent onHit;

        private Health target = null;
        private Health owner = null;
        float damage = 0;

        private void OnTriggerEnter(Collider other) {
            Health targetEnter = other.GetComponent<Health>();
            if(targetEnter == null) return;
            if(targetEnter.IsDead()) {
                autoGuidance = false;
                return;
            }
            if(targetEnter != owner){
                onHit.Invoke();
                targetEnter.TakeDamage(owner.gameObject,damage);
                if(impactEffect != null) {
                    GameObject effect = Instantiate(impactEffect, GetAimLocation(), transform.rotation);
                }
                foreach(GameObject toDestroy in destroyOnHit){
                    Destroy(toDestroy);
                }
                Destroy(gameObject, lifeAfterImpact);
                speed = 0;
            }
        }

        private void Update() {
            Fly();
        }

        public void SetTarget(Health target, float damage, Health owner){
            this.target = target;
            this.damage = damage;
            this.owner = owner;
            transform.LookAt(GetAimLocation());
            Destroy(gameObject, maxLifetime);
        }

        private void Fly()
        {
            if (target == null) return;
            if(autoGuidance){
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null) {
                Debug.LogError("The target object do not have a capsule collider");
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height*2/3;
        }
    }
}
