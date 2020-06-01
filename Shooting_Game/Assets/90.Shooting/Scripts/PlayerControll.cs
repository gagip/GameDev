using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    // 플레이어 이동속도
    public float speed = 15.0f;
    // 플레이어 게임오브젝트의 트랜스폼 컴포넌트
    private Transform myTransform = null;
    // 플레이어가 생성하게 될 불렛프리팹
    public GameObject bulletPrefab = null;
    
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // 이동
        // -1 ~ 1 L 왼쪽 화살키(-1) 오른쪽 화살키(1)
        float axis = Input.GetAxis("Horizontal");
        //Debug.Log("axis :" + axis);
        // 매프레임당 이 게임 오브젝트가 원하는 속도와 방향으로 이동하는 양
        Vector3 moveAmount = axis * speed * -Vector3.right * Time.deltaTime;

        myTransform.Translate(moveAmount);

        // 슈팅
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, myTransform.position, Quaternion.identity);
        }

    }
}
