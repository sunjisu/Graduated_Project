using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // 카메라 관련 컴포넌트
    [SerializeField] private Camera theCamera; // 카메라 세팅
    [SerializeField] private float cameraMoving; // 카메라 움직임
    [SerializeField] private float cameraLimit; // 카메라 움직임 제한

    private float currentCameraRotateX; // 카메라 상하 회전

   
    private void FixedUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        float rotationX = Input.GetAxisRaw("Mouse Y"); // 상하 이동
        float cameraRotationX = rotationX * cameraMoving; // 카메라 상하 조절

        currentCameraRotateX -= cameraRotationX;
        currentCameraRotateX = Mathf.Clamp(currentCameraRotateX, -cameraLimit, cameraLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotateX, 0f, 0f);
    }

}
