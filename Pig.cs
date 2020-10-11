using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour {

    [SerializeField] private string animalName; // 동물의 이름
    [SerializeField] private int hp; // 동물의 체력

    [SerializeField] private float walkSpeed; // 걷기 스피드
    [SerializeField] private float runSpeed; // 달리기 스피드

    private Vector3 direction; // 방향 정하기

    private bool isAction; // 행동중인가 체크
    private bool isWalking; // 걷기 체크
    private bool isRunning; // 뛰기 체크

    [SerializeField] private float walkTime; // 걷는 시간
    [SerializeField] private float behaviorTime; // 행동 시간
    [SerializeField] private float runTime; // 달리기 시간
    private float currentTime; // 시간 체크

    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;



    // Use this for initialization
    void Start () {
        currentTime = behaviorTime;
        isAction = true;
    }
	
	// Update is called once per frame
	void Update () {
        Move(); // 걷기 함수
        Rotate(); // 회전 함수
        TimeCheck(); // 시간 체크함수
    }

    private void Move()
    {
        if (isWalking) // 걷는중일경우
        {
            // 이 속도로 계산해서 앞으로 걸어감
            rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
        }
        else if(isRunning) // 뛰는중일경우
        {
            rigid.MovePosition(transform.position + (transform.forward * runSpeed * Time.deltaTime));
        }
    }

    private void Rotate()
    {
        if (isWalking || isRunning) // 움직이는 중일경우
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y ,0f), 0.01f);
            rigid.MoveRotation(Quaternion.Euler(_rotation)); // 회전
        }
    }

    private void TimeCheck()
    {
        if(isAction) // 행동중이라면
        {
            currentTime -= Time.deltaTime; // 매 초마다 계산
            if(currentTime <= 0) // 시간이 0이 되었거나 작아졌다면
            {
                Init(); // 행동 초기화
            }
        }
    }

    private void Init()
    {
        isWalking = false;
        isRunning = false;
        isAction = true;

        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        direction.Set(0f, Random.Range(0f, 360f), 0f); // 0 ~ 360 까지 랜덤 방향 지정
        RandomAction();
    }

    private void RandomAction()
    {
        int actionCheck;

        actionCheck = Random.Range(0, 4); // 행동 패턴 정하기

        switch (actionCheck)
        {
            case 0:
                Wait();
                break;
            case 1:
                Walk();
                break;
            case 2:
                Peek();
                break;
            case 3:
                Eat();
                break;
        }
    }

    private void Wait()
    {
        currentTime = behaviorTime;

    }
    
    private void Walk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;

    }

    private void Peek()
    {
        anim.SetTrigger("Peek");
        currentTime = behaviorTime;
    }

    private void Eat()
    {
        anim.SetTrigger("Eat");
        currentTime = behaviorTime;
    }

    private void Run(Vector3 _targetPos) // 공격 받았을때 달리기
    {
        direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles; // 플레이어와 반대쪽 바라봄

        currentTime = runTime; // 뛰기 시간 체크
        isWalking = false; 
        isRunning = true; 

        anim.SetBool("Running", isRunning);
    }

    public void Damage(int _dmg, Vector3 _targetPos)
    {
        hp -= _dmg;
        

        if(hp <= 0)
        {
            anim.SetTrigger("Dead");
        }

        anim.SetTrigger("Hurt");
        Run(_targetPos);
    }
}
