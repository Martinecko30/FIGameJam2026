using System.Collections;
using FPSDemo.Target;
using FPSDemo.Utils;
using UnityEngine;

namespace FPSDemo.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public float gunShotDistanceToAlert;
        [SerializeField] AudioSource weaponAudioSource;

        public float fireRate = 3f;
        public float maxRange = 1000f;
        public float angleSpreadPerShot = 10f;
        public float defaultHipFireAngleSpread = 1f;
        public float maxAngleSpreadWhenShooting = 10f;
        public AnimationCurve spreadStabilityGain;
        [SerializeField] AudioClip[] shotSFX;
        [SerializeField] AudioClip[] hitSFX;
        [SerializeField] [Range(0f, 1f)] float hitSFXVolume = 1f;
        [SerializeField] GameObjectPooler bloodHitPooler;
        [SerializeField] GameObjectPooler bulletHoleVFX;

        [Header("Positioning")] public Vector3 normalPos;
        public Vector3 normalRot;
        [Space(10)] public Vector3 blockingPos;
        public Vector3 blockingRot;
        [Space(10)] public Vector3 runPos;
        public Vector3 runRot;
        [Space(10)] public Vector3 awayPos;
        public Vector3 awayRot;

        [Header("Recoil")] public Vector3 recoil;
        public float blowbackForce;
        public float recoilRecoverSpeed;

        public void Fire(HumanTarget target, Transform bulletStart, LayerMask shotLayerMask, int ragdollBodyLayerIndex)
        {
            weaponAudioSource.PlayOneShot(shotSFX[Random.Range(0, shotSFX.Length)]);

            if (Physics.Raycast(bulletStart.position, bulletStart.forward, out RaycastHit hit, maxRange, shotLayerMask))
            {
                MakeImpactVFX(hit);
                if (hitSFX != null && hitSFX.Length > 0)
                    AudioSource.PlayClipAtPoint(hitSFX[Random.Range(0, hitSFX.Length)], hit.point, hitSFXVolume);
            }
            
            var healthSystem = hit.collider?.GetComponentInParent<HealthSystem>();                                                                                                                      
            if (healthSystem != null)                                                                                                                                                             
            {                                                                                                                                                                                 
                healthSystem.WasShot(target);                                                                                                                  
            } 
        }

        void MakeImpactVFX(RaycastHit hit)
        {
            GameObject impactVFXGO;
            
            switch (hit.transform.gameObject.layer)
            {
                case LayerManager.ragdollBodyLayer:
                    if (bloodHitPooler == null)
                    {
                        bloodHitPooler = GameObject.FindGameObjectWithTag("BloodHitPooler")
                            .GetComponent<GameObjectPooler>();
                    }
                    impactVFXGO = bloodHitPooler.GetPooledGO();
                    break;
                default:
                    if (bulletHoleVFX == null)
                    {
                        bulletHoleVFX = GameObject.FindGameObjectWithTag("BulletHolePooler")
                            .GetComponent<GameObjectPooler>();
                    }
                    impactVFXGO = bulletHoleVFX.GetPooledGO();
                    break;
            }

            impactVFXGO.transform.position = hit.point;
            impactVFXGO.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }
}
