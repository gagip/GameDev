using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour {

    public string tag = string.Empty;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tag) == true)
        {
            gameObject.SendMessageUpwards("OnSetTarget", other.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
