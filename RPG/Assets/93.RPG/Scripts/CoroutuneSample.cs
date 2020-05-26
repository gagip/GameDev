using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutuneSample : MonoBehaviour {

    private IEnumerator Start()
    {
        yield return StartCoroutine(TestCoroutine());
    }

    IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
