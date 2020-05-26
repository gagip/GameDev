using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // 플레이어 비행체의 이동속도
    public float speed = 15.0f;
    // 플레이어 게임오브젝트의 트렌스폼 컴포넌트
    private Transform myTransform = null;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // -1 ~ 1 
        float axis = Input.GetAxis("Horizontal");
        Vector3 moveAmount = axis * speed * -Vector3.right * Time.deltaTime;

        myTransform.Translate(moveAmount);

        // 스페이스바 키가 눌렸다면
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            Instantiate(bulletPrefab, myTransform.position, Quaternion.identity);
        }
    }
}
