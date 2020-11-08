using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    // 플레이어 컴포넌트
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private int hp; // 체력

    private float applySpeed; // 속도 넣어줌


    private bool isDamage = false;
    private bool isRun = false;
    private bool isGround; // 땅에 붙어 있는가?


    [SerializeField] private float playerRotateLimit; // 캐릭터 회전 제한

    GunController theGunController;

    private CapsuleCollider myCol;
    private Rigidbody myRigid; 


	// Use this for initialization
	void Start () {
        myCol = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        theGunController = FindObjectOfType<GunController>();
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        IsGround();
        MoveState();
        Move();
        PlayerRotation();
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, myCol.bounds.extents.y + 0.2f);
    }

    private void MoveState()
    {
        if(Input.GetKey(KeyCode.LeftShift)) // 왼쪽 쉬프트가 눌려있을때 실행
        {
            theGunController.CancelAiming(); // 뛰면 정조준모드 해제

            isRun = true;
            applySpeed = runSpeed;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) // 왼쪽 쉬프트에서 떨어졌을때 실행
        {
            isRun = false;
            applySpeed = walkSpeed;
           
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround) // 스페이스가 눌렸을때 실행
        {

            isGround = false;
            myRigid.velocity = transform.up * jumpPower;
        }

    }

    private void PlayerRotation()
    {
        float rotationY = Input.GetAxisRaw("Mouse X");

        Vector3 playerRotationY = new Vector3(0f, rotationY, 0f) * playerRotateLimit;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(playerRotationY));
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal"); // 좌우 이동
        float moveDirZ = Input.GetAxisRaw("Vertical"); // 상하 이동

        Vector3 moveHorzontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 movePos = (moveHorzontal + moveVertical).normalized * applySpeed; // 움직임 체크

        myRigid.MovePosition(transform.position + movePos * Time.deltaTime); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                hp -= enemyBullet.damage;
                StartCoroutine(OnDamageCoroutine());
            }          
        }
    }

    IEnumerator OnDamageCoroutine()
    {
        isDamage = true;

        yield return new WaitForSeconds(1f);

        isDamage = false;
    }

}
