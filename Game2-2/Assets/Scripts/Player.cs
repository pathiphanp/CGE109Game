using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, TakeDamage<float>
{
    CharacterController controller;
    Animator anim;

    [Header("Move")]
    [SerializeField] float speed;

    [Header("Sensitivity")]
    [SerializeField] float zoomSpeed;
    [SerializeField] float _mouseX_Sensitivity;
    [SerializeField] float _mouseY_Sensitivity;
    [SerializeField] bool canlookAround = true;
    float mouseX;
    float mouseY;
    float _rotationY;
    float _rotationX;

    [Header("Out_Of_Focus")]
    Quaternion lastRotation;
    Vector3 lastTranfrom;


    [Header("Allstatus")]
    [SerializeField] public float hp;

    [Header("TimeStop")]
    [SerializeField] float slowTime;

    float time = 1;
    [Header("Enemy")]
    [SerializeField] bool lookEnemy;
    [SerializeField] float speedDuration;
    Vector3 relativePosition;
    Quaternion enemyRotation;
    float speedLookEnemy;

    [Header("CheckEnemyAround")]
    [SerializeField] GameObject enemy;
    [SerializeField] Collider[] allEnemy;
    [SerializeField] float[] _distance;
    float tamp_distance;
    [SerializeField] bool canFindEnemy = true;
    [SerializeField] float radius;
    public GameObject findEnemy;
    [SerializeField] LayerMask layer;
    [SerializeField] float maxDistance;
    [SerializeField] GameObject[] left;
    [SerializeField] int leftIndex;
    [SerializeField] GameObject[] right;
    [SerializeField] int rightIndex;
    float[] dot;
    Vector3 dirToTarget;

    [Header("Camera")]
    public GameObject myCamera;
    [SerializeField] float maxClamp;
    [SerializeField] float minClamp;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        this.transform.InverseTransformPoint(enemy.transform.position);
        Debug.Log(this.transform.InverseTransformPoint(enemy.transform.position));
        ControllerMove();
        StopGame();
        LookEnemy();
        LookAround();
        CheckEnemyAround();
    }

    void Gravity()
    {

    }
    void ControllerMove()
    {
        float horizontrol = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = ((transform.right * horizontrol + transform.forward * vertical) + new Vector3(0, 0, 0)).normalized;
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }
        anim.SetFloat("MoveWalk", vertical);
    }
    void TakeDamage<float>.TakeDamage(float damage)
    {
        hp -= damage;
    }
    void StopGame()
    {
        if (Input.GetKeyDown("e"))
        {
            if (time == 1)
            {
                time = slowTime;
            }
            else if (time == slowTime)
            {
                time = 1;
            }
            Time.timeScale = time;
        }
    }

    void LookEnemy()
    {
        leftIndex = Mathf.Clamp(leftIndex, 0, allEnemy.Length - 1);
        rightIndex = Mathf.Clamp(rightIndex, 0, allEnemy.Length - 1);
        lastRotation = transform.rotation;
        lastTranfrom = transform.position;
        if (Input.GetKeyDown("q"))
        {
            lookEnemy = !lookEnemy;
            canlookAround = !canlookAround;
            speedLookEnemy = 0;
            canFindEnemy = !canFindEnemy;
        }
        if (lookEnemy)
        {
            speedLookEnemy += Time.deltaTime;
            float percentComlete = speedLookEnemy / speedDuration;
            relativePosition = enemy.transform.position - transform.position;
            relativePosition = new Vector3(relativePosition.x, 0, relativePosition.z);
            enemyRotation = Quaternion.LookRotation(relativePosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, enemyRotation, percentComlete);
        }
    }
    void LookAround()
    {
        if (canlookAround)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _mouseX_Sensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _mouseY_Sensitivity;
            _rotationY += mouseX;
            _rotationX -= mouseY;
            transform.RotateAround(transform.position, Vector3.up, mouseX);
            /*_rotationX = Mathf.Clamp(_rotationX, minClamp, maxClamp);
            myCamera.transform.localEulerAngles = new Vector3(_rotationX,0, 0);*/
        }
    }

    void CheckEnemyAround()
    {
        allEnemy = Physics.OverlapSphere(findEnemy.transform.position, radius, layer);
        left = new GameObject[allEnemy.Length];
        right = new GameObject[allEnemy.Length];
        _distance = new float[allEnemy.Length];
        dot = new float[allEnemy.Length];

        //หาตัวระยะห่างทุกตัว
        for (int i = 0; i < allEnemy.Length; i++)
        {
            _distance[i] = Vector3.Distance(transform.position, allEnemy[i].gameObject.transform.position);
        }
        //ค่าของระทางแรกที่ไปใช้วัด
        tamp_distance = _distance[0];
        //หาระยะทางที่ไกล้ที่สุด
        if (canFindEnemy)
        {
            for (int i = 0; i < allEnemy.Length; i++)
            {
                /*Debug.Log("ระยะทางที่เข้ามา" + " : " + _distance[i]);
                Debug.Log("ระยะทางตัวต้นที่ใช่วัด" + " : " + tamp_distance);*/
                if (tamp_distance >= _distance[i])
                {
                    tamp_distance = _distance[i];
                    enemy = allEnemy[i].gameObject;
                    /*Debug.Log("หาผู้ที่มีระยะทางน้อยสุด" + " : " + _distance[i]);
                    Debug.Log("ผู้ที่มีระยะน้อยสุด" + " : " + tamp_distance);*/
                }
            }
        }

        //หาว่าใครอยู่ซ้ายกับขวา
        for (int i = 0; i < allEnemy.Length; i++)
        {
            dirToTarget = Vector3.Normalize(transform.position - allEnemy[i].transform.position);
            dot[i] = Vector3.Dot(transform.right, dirToTarget);
            if (dot[i] > 0)
            {
                left[i] = allEnemy[i].gameObject;
            }
            else if (dot[i] < 0)
            {
                right[i] = allEnemy[i].gameObject;
            }
        }
    }
}
