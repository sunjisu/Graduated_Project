using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField] private Gun theGun;
    [SerializeField] private Vector3 originPos; // 원래 위치

    private float currentAttackSpeed;

    private bool isReload = false; // 재장전 중인가 체크
    private bool isAiming = false; // 정조준중인가 체크

    private AudioSource audioSource;

    private RaycastHit hitInfo; // 공격 대상 체크



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update () {
        AttackSpeedCheck(); // 공격 속도 체크 함수
        Attack(); // 공격 함수
        ReloadExecution(); // 재장전 실행 함수
        Aiming(); // 정조준 실행 함수
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

                    Hit();

                    StopAllCoroutines();
                    StartCoroutine(ReboundCourutine());// 총기 반동

                    Debug.Log("총알 발사");
                }
                else
                {
                    StartCoroutine(ReloadCoroutine());
                }
            }
           
        }
    }

    private void Hit()
    {
        if (HitCheck())
        {
            if (hitInfo.transform.tag == "NPC")
            {                
                hitInfo.transform.GetComponent<Pig>().Damage(theGun.power, transform.position);
            }
            else if (hitInfo.transform.tag == "Enemy A")
            {
                hitInfo.transform.GetComponent<Enemy>().Damage(theGun.power, transform.position);
            }
            else if(hitInfo.transform.tag == "Enemy C")
            {
                hitInfo.transform.GetComponent<Enemy>().Damage(theGun.power, transform.position);
            }
            Debug.Log(hitInfo.transform.name);
        }
    }

    private bool HitCheck()
    {
        // 충돌한게 있다면
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, theGun.attackRange))
        {
            return true;
        }

        return false;
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

            theGun.carryBulletCount += theGun.currentBulletCount; // 장전되어 있던 총알을 탄알집으로 돌리고
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
        else
        {
            ;
        }
    }

    private void Aiming()
    {
        if(Input.GetButtonDown("Fire2") && !isReload) // 마우스 오른쪽키가 눌렸을때
        {
            isAiming = !isAiming;
            theGun.anim.SetBool("Aiming", isAiming);

            if(isAiming)
            {
                StopAllCoroutines(); // 모든 코루틴 멈춤
                StartCoroutine(AimingCorutine());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(NormalCorutine());
            }
        }
      
    }

    IEnumerator ReboundCourutine()
    {
        Vector3 maxRebound = new Vector3(theGun.rebound, originPos.y , originPos.z);
        Vector3 maxAimingRebound = new Vector3(theGun.aimingRebound, theGun.aimingPos.y, theGun.aimingPos.z);

        if(!isAiming)
        {
            theGun.transform.localPosition = originPos; // 원래 위치로 되돌림

            while(theGun.transform.localPosition.x <= theGun.rebound - 0.02f) // 반동
            {
                theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, maxRebound, 0.4f);

                yield return null;
            }

            while(theGun.transform.localPosition != originPos) // 원래 위치로 돌아옴
            {
                theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, originPos, 0.1f);

                yield return null;
            }
        }
        else
        {
            theGun.transform.localPosition = theGun.aimingPos; // 정조준 위치로 되돌림

            while (theGun.transform.localPosition.x <= theGun.aimingRebound - 0.02f) // 반동
            {
                theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, maxAimingRebound, 0.4f);

                yield return null;
            }

            while (theGun.transform.localPosition != theGun.aimingPos) // 원래 위치로 돌아옴
            {
                theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, theGun.aimingPos, 0.1f);

                yield return null;
            }
        }

    }

    IEnumerator AimingCorutine() // 정조준까지 자연스럽게 이동
    {
        while(theGun.transform.localPosition != theGun.aimingPos) // 정조준 상태위치가 현재 위치가 같지 않을때
        {
            theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, theGun.aimingPos , 0.2f);
            yield return null; // 1 프레임 대기
        }
    }

    IEnumerator NormalCorutine() // 일반 상태까지 자연스럽게 이동
    {
        while (theGun.transform.localPosition != originPos) // 일반상태위치에 총이 있지 않을때
        {
            theGun.transform.localPosition = Vector3.Lerp(theGun.transform.localPosition, originPos, 0.2f);
            yield return null; // 1 프레임 대기
        }
    }

    public void CancelAiming()
    {
        if(isAiming)
        {
            isAiming = !isAiming;
            theGun.anim.SetBool("Aiming", isAiming);

            if (isAiming)
            {
                StopAllCoroutines(); // 모든 코루틴 멈춤
                StartCoroutine(AimingCorutine());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(NormalCorutine());
            }
        }
    }


    private void PlaySound(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return theGun;
    }
    
}
