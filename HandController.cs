using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    [SerializeField] private Hand theHand;

    private bool isAttack = false;


    private RaycastHit hitInfo; // 공격 대상 체크

  
	
	// Update is called once per frame
	void Update () {

        Attack();
		
	}

    private void Attack()
    {
        if(Input.GetButton("Fire1")) // 마우스 왼쪽키가 눌려있을때 실행
        {
            if(!isAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        theHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(theHand.attackStart); // 공격 지속 시간       

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(theHand.attackEnd); // 공격 끝날때 까지의 시간       

        yield return new WaitForSeconds(theHand.attackSpeed - theHand.attackStart - theHand.attackEnd); // 공격 스피드 체크

        isAttack = false;
    }

    IEnumerator HitCoroutine()
    {        
        if(HitCheck())
        {
            if(hitInfo.transform.tag == "NPC")
            {
                hitInfo.transform.GetComponent<Pig>().Damage(theHand.power, transform.position);
            }            
            Debug.Log(hitInfo.transform.name);      
        }
        yield return null;
       
    }

    private bool HitCheck()
    {
        // 충돌한게 있다면
        if(Physics.Raycast(transform.position, transform.forward , out hitInfo , theHand.attackRange))
        {
            return true;
        }

        return false;
    }
}
