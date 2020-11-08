using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{

    [SerializeField] private string enemyName; // 동물의 이름
    [SerializeField] private int hp; // 적의 체력
    private int currentHp;
    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider meleeArea;

    public Image hpbar;



    public bool isChase; // 추격 체크
    public bool isAttack; // 공격 상태 체크

    private bool isDead = false; // 죽음상태

    private Animator anim;
    private Rigidbody rigid;
    private BoxCollider boxCol;
    
    Material mat;
    NavMeshAgent nav;

    void Start()
    {      
        rigid = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        currentHp = hp;

        Invoke("ChaseStart", 2);

        InitHpBarSize(); 

    }


    void Update()
    {
        if(nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        hpbar.rectTransform.localScale = new Vector3((float)currentHp / (float) hp, 1f, 1f);
    }

    private void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }

    private void InitHpBarSize()
    {
        hpbar.rectTransform.localScale = new Vector3(1f, 1f, 1f); // hp 바 세팅
    }

    private void Targetting()
    {
        float targetRadius = 0.5f;
        float targetRange = 1.5f;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius,
                                                     transform.forward, targetRange,
                                                     LayerMask.GetMask("Player")); // 플레이어에 닿으면 

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(AttackCoroutine()); // 공격실행
        }
    }

    IEnumerator AttackCoroutine()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("Attack", true);
       
        yield return new WaitForSeconds(0.2f);

        meleeArea.enabled = true;

        yield return new WaitForSeconds(1f);
        
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        isChase = true;
        isAttack = false;
        anim.SetBool("Attack", false);
    }

    private void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    private void ChaseStart()
    {
        isChase = true;
        anim.SetBool("Walk" , true);
    }

    
    public void Damage(int _dmg, Vector3 _playerPos)
    {
        if (!isDead)
        {
            currentHp -= _dmg;


            if (currentHp <= 0)
            {
                Debug.Log("죽음");
                anim.SetTrigger("Die");

                isChase = false;
                nav.enabled = false;

                isDead = true;

                Destroy(gameObject, 3);

                return;
            }         
            
        }
    }
    
}
