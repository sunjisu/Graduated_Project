using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField] private Gun theGun;

    private float currentAttackSpeed;

    private bool isReload = false; // 재장전 중인가 체크

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update () {
        AttackSpeedCheck();
        Attack();
        ReloadExecution();

    }

    private void AttackSpeedCheck()
    {
        if(currentAttackSpeed > 0)
        {
            currentAttackSpeed -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        if (Input.GetButton("Fire1") && currentAttackSpeed <= 0 && !isReload)
        {
            if(!isReload)
            {
                if (theGun.currentBulletCount > 0)
                {
                    theGun.currentBulletCount--;

                    currentAttackSpeed = theGun.attackSpeed; // 공격 속도 계산

                    PlaySound(theGun.gunSound);
                    theGun.effect.Play();
                    Debug.Log("총알 발사");
                }
                else
                {
                    StartCoroutine(ReloadCoroutine());
                }
            }
           
        }
    }

    private void ReloadExecution()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && theGun.currentBulletCount < theGun.reloadBulletCount)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if(theGun.carryBulletCount > 0) // 소지하고 있는 총알수가 0보다 크면
        {
            isReload = true;

            theGun.anim.SetTrigger("Reload");

            theGun.carryBulletCount += theGun.currentBulletCount;
            theGun.currentBulletCount = 0;


            yield return new WaitForSeconds(theGun.reloadSpeed);

            if(theGun.carryBulletCount >= theGun.reloadBulletCount) // 보유한 총알이 재장전 가능개수보다 많을경우
            {
                theGun.currentBulletCount = theGun.reloadBulletCount;
                theGun.carryBulletCount -= theGun.reloadBulletCount;
            }
            else
            {
                theGun.currentBulletCount = theGun.carryBulletCount;
                theGun.carryBulletCount = 0;
            }


            isReload = false;
        }
    }

    private void PlaySound(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
