using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float enemySpeed = 50.0f;

    private Transform myTransform;

    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = enemySpeed * Vector3.back * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if(myTransform.position.y < -50.0f)
        {
            InitPosition();
        }
    }



    void InitPosition()
    {
        myTransform.position = new Vector3(Random.Range(-60.0f, 60.0f), 50.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 총알에 맞았다면
        if(other.tag == "bullet")
        {
            MainControl.score += 100;

            Instantiate(explosion, myTransform.position, Quaternion.identity);

            Debug.Log("Bullet Trigger Enter");
            InitPosition();
            Destroy(other.gameObject);
        }
    }
}
