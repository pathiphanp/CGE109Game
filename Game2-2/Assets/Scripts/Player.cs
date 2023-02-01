using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,TakeDamage<float>
{
    Camera cam;
    public float tragetZoom;
    public float zoomfactor;
    public float zoomLarpSpeed;
    [SerializeField] public float hp;
    void Awake()
    {
        cam = Camera.main;
        tragetZoom = cam.orthographicSize;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            tragetZoom = 5f;
        }
        else if(Input.GetMouseButtonDown(1))
        {
            tragetZoom = 10f;
        }
        
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,tragetZoom,Time.deltaTime* zoomLarpSpeed);
    }
    void TakeDamage<float>.TakeDamage(float damage)
    {
        hp -= damage;
    }
   
}
