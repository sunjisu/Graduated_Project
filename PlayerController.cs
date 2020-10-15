using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    // 플레이어 컴포넌트
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float playerRotateLimit; // 캐릭터 회전 제한

    private Rigidbody myRigid; 


	// Use this for initialization
	void Start () {
        myRigid = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
        PlayerRotation();
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

        Vector3 movePos = (moveHorzontal + moveVertical).normalized * walkSpeed; // 움직임 체크

        myRigid.MovePosition(transform.position + movePos * Time.deltaTime); 
    }


}
