using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeDrop : MonoBehaviour
{
    public float floorHeight;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Drop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Drop()
    {
        float t = 0.3f;

        while(t > 0 || transform.position.y > floorHeight)
        {
            transform.Translate(Vector2.down * 0.05f);

            t -= Time.deltaTime;

            yield return null;
        }



        yield return null;
    }
}
