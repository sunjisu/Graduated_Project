using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {


    // 플레이어 컴포넌트
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpPower;
    private int currentHp;
    [SerializeField] private int hp; // 체력


    private float applySpeed; // 속도 넣어줌


    private bool isDamage = false;
    private bool isWalk = false;
    private bool isRun = false;
    private bool isGround = true; // 땅에 붙어 있는가?

    private Vector3 lastPos; // 이전 프레임 위치


    [SerializeField] private float playerRotateLimit; // 캐릭터 회전 제한

    private Crosshair theCrosshair;
    GunController theGunController;

    public Image hpbar;
    public Text hp_text;

    private CapsuleCollider myCol;
    private Rigidbody myRigid; 


	// Use this for initialization
	void Start () {
        myCol = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();        
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();

        applySpeed = walkSpeed;
        currentHp = hp; // 현재 hp에 최대 hp넣어줌
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        IsGround();
        MoveState();
        Move();
        MoveCheck();
        PlayerRotation();

        hp_text.text = hp.ToString();
        hpbar.rectTransform.localScale = new Vector3((float)hp / (float)currentHp, 1f, 1f);
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
            theCrosshair.RunningAnimation(isRun);
            applySpeed = runSpeed;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) // 왼쪽 쉬프트에서 떨어졌을때 실행
        {
            isRun = false;
            theCrosshair.RunningAnimation(isRun);
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

    private void MoveCheck()
    {
        if (!isRun && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f) // 마지막 위치와 현재위치의 차이가 0.01보다 크다면
            {
                isWalk = true;

            }
            else
            {
                isWalk = false;
            }
            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                hp -= enemyBullet.damage;
                if(other.GetComponent<Rigidbody>() != null) // 총알이 닿았을때 그 총알 파괴
                {
                    Destroy(other.gameObject);
                }

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
