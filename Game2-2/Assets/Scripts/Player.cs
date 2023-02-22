using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, TakeDamage<float>
{
    CharacterController controller;
    Animator anim;

    [Header("Move")]
    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float spinSpeed;


    [Header("Sensitivity")]
    [SerializeField] float zoomSpeed;
    [SerializeField] float _mouseX_Sensitivity;
    [SerializeField] float _mouseY_Sensitivity;
    [SerializeField] bool canlookAround = true;
    float mouseX;
    float mouseY;
    float _rotationY;
    float _rotationX;

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
    [SerializeField] GameObject findNextEnemy;
    [SerializeField] Vector3[] nextEnemy;

    Vector3 o = Vector3.zero;

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
        findNextEnemy = new GameObject("find next enemy");
    }
    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        ControllerMove();
        StopGame();
        LookEnemy();
        LookAround();
        CheckEnemyAround();
        Change();
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
            if (Input.GetKeyDown("left shift"))
            {
                speed *= 2;
            }
            else if (Input.GetKeyUp("left shift"))
            {
                speed = maxSpeed;
            }
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
        //ทำการกดปุ่มเพื่อมองหาศัตรูที่ไกล้ที่สุด
        if (enemy != null)
        {
            if (Input.GetKeyDown("q"))
            {
                lookEnemy = !lookEnemy;
                canlookAround = !canlookAround;
                speedLookEnemy = 0;
                canFindEnemy = !canFindEnemy;
            }
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
        allEnemy = Physics.OverlapSphere(findEnemy.transform.position, radius, layer);//เช็คจำนวนEnemy
        _distance = new float[allEnemy.Length];
        if (allEnemy.Length != 0)//เช็คว่ามีศัตรู
        {
            if (allEnemy[0] != null)
            {
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
            }
        }
        else
        {
            enemy = null;
        }
    }
    void Change()
    {
        if (enemy != null)
        {
            findNextEnemy.transform.position = enemy.transform.position;
            findNextEnemy.transform.localRotation = enemy.transform.localRotation;
        }
        nextEnemy = new Vector3[allEnemy.Length];
        // xติดลบ = :ซ้าย || xเป็นบวก = ขวา
        // zบวก = หน้า || zลบ = หลัง
        // z ไกล้ 0 จะมีค่ามากกว่า 
        //หาว่า X ใครไกล้กว่า จ่านั้น เช็คว่า Y ใครไกล้กว่า
        for (int i = 0; i < allEnemy.Length; i++)
        {
            nextEnemy[i] = findNextEnemy.transform.InverseTransformPoint(allEnemy[i].gameObject.transform.position);
            //เอา nextEnemy.x,.z มาเปรียบเทียบกับตัวอื่น
            //ตัวที่ 1 มาเทียบตัวที่ 2
            //i 0 เปรียบเทียบ i 1
            int last_i = 0;
            if (nextEnemy[i].x < 0 && nextEnemy[i].z > nextEnemy[last_i].z && nextEnemy[i].x != 0)
            {
                Debug.Log("Left : " + allEnemy[last_i].gameObject.name + nextEnemy[i]);
            }
            last_i++;
            /*if (nextEnemy[i].x > 0 && nextEnemy[i].x != 0)
            {
                Debug.Log("Right  : " + allEnemy[i].gameObject.name + nextEnemy[i]);
            }*/
            if (Input.GetAxisRaw("Mouse X") < -5)//สปัดเม้าไปทางซ้าย
            {

            }
            else if (Input.GetAxisRaw("Mouse X") > 5)//สปัดเม้าไปทางขวา
            {

            }
        }
    }
}
