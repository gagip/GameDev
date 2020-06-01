using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float bulletSpeed = 100.0f;
    private Transform myTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = bulletSpeed * Vector3.up * Time.deltaTime;
        myTransform.Translate(moveAmount);

        // 화면밖으로 나가면
        if (myTransform.position.y > 60.0f)
        {
            Destroy(gameObject);
        }
    }
}
