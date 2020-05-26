using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [Header("카메라기본속성")]
    private Transform myTransform;
    public GameObject target;
    private Transform targetTransform;
    
    public enum CameraViewPointState { FIRST, SECOND, THIRD }
    public CameraViewPointState cameraState = CameraViewPointState.THIRD;

    [Header("3인칭 카메라")]
    public float distance = 5.0f; // 타겟으로부터 떨어진 거리
    public float height = 1.5f; // 타겟의 위치보다 더 추가적인 높이
    // damping값은 사용자가 멀미하는 것을 방지하는데 의의
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    [Header("2인칭 카메라")]
    public float rotateSpeed = 20.0f;

    [Header("1인칭 카메라")]
    public float sensitivityX = 5.0f;
    public float sensitivityY = 5.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public Transform firstCameraSocket = null;

    private void Start()
    {
        myTransform = GetComponent<Transform>();
        if(target != null)
        {
            targetTransform = target.transform;
        }
    }

    /// <summary>
    /// 3인칭 카메라
    /// </summary>
    void ThirdView()
    {
        float wantedRotationAngle = targetTransform.eulerAngles.y; // 현재 타겟의 y축 각도 값
        float wantedHeight = targetTransform.position.y + height; // 현재 타겟의 높이 + 우리가 추가로 높이고 싶은 높이

        float currentRotationAngle = myTransform.eulerAngles.y; // 현재 카메라의 y축 각도 값
        float currentHeight = myTransform.position.y; // 현재 카메라의 높이
        // 현재 각도에서 원하는 각도로 댐핑값을 얻게 됩니다
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle,
                                                rotationDamping * Time.deltaTime);
        // 현재 높이에서 원하는 높이로 댐핑값을 얻게 됩니다
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight,
                                    heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

        myTransform.position = targetTransform.position;
        myTransform.position -= currentRotation * Vector3.forward * distance;
        myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);

        myTransform.LookAt(targetTransform);
    }

    /// <summary>
    /// 모델뷰
    /// </summary>
    void SecondView()
    {
        myTransform.RotateAround(targetTransform.position, Vector3.up, rotateSpeed * Time.deltaTime);
        myTransform.LookAt(targetTransform);
    }

    /// <summary>
    /// 1인칭 뷰
    /// </summary>
    void FirstView()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = myTransform.localEulerAngles.y + mouseX * sensitivityX;
        // 마이너스 각도를 조절하기 위한 연산
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;

        rotationY = rotationY + mouseY * sensitivityY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;

        myTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f); // 마우스 좌우 이동은 y축 회전, 상하 이동은 x축 회전

        myTransform.position = firstCameraSocket.position;
    }

    /// <summary>
    /// update함수 후에 호출되는 업데이트
    /// </summary>
    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }
        if(targetTransform == null)
        {
            targetTransform = target.transform;
        }
        switch (cameraState)
        {
            case CameraViewPointState.THIRD:
                ThirdView();
                break;
            case CameraViewPointState.SECOND:
                SecondView();
                break;
            case CameraViewPointState.FIRST:
                FirstView();
                break;
        }
    }
}
