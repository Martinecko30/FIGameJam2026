using System.Collections;
using FPSDemo.Target;
using FPSDemo.Utils;
using UnityEngine;

namespace FPSDemo.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public float gunShotDistanceToAlert;
        [SerializeField] Transform swingFXPos;
        [SerializeField] float muzzleFlashDuration = 0.15f;
        [SerializeField] AudioSource weaponAudioSource;

        public float fireRate = 3f;
        public float maxRange = 1000f;
        public float angleSpreadPerShot = 10f;
        public float defaultHipFireAngleSpread = 1f;
        public float maxAngleSpreadWhenShooting = 10f;
        public AnimationCurve spreadStabilityGain;
        [SerializeField] AudioClip[] shotSFX;
        [SerializeField] GameObjectPooler muzzleFlashVFX;
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
            MakeMuzzleFlash();

            if (Physics.Raycast(bulletStart.position, bulletStart.forward, out RaycastHit hit, maxRange, shotLayerMask))
            {
                MakeImpactVFX(hit);
            }
        }

        void MakeMuzzleFlash()
        {
            GameObject flash = muzzleFlashVFX.GetPooledGO();
            StartCoroutine(MaintainMuzzleFlashPositionAndRotation(flash));
        }

        IEnumerator MaintainMuzzleFlashPositionAndRotation(GameObject flash)
        {
            float timeLeft = muzzleFlashDuration;
            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                flash.transform.position = swingFXPos.position;
                flash.transform.rotation = swingFXPos.rotation;
                yield return null;
            }

            yield return new WaitForEndOfFrame(); // TODO needed?
            flash.SetActive(false);
        }

        void MakeImpactVFX(RaycastHit hit)
        {
            GameObject impactVFXGO;
            switch (hit.transform.gameObject.layer)
            {
                case LayerManager.ragdollBodyLayer:
                    impactVFXGO = bloodHitPooler.GetPooledGO();
                    break;
                default:
                    impactVFXGO = bulletHoleVFX.GetPooledGO();
                    break;
            }

            impactVFXGO.transform.position = hit.point;
            impactVFXGO.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }
}
