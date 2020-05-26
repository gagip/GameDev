using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControl : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    public Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        myRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, Time.time * scrollSpeed));
    }
}
