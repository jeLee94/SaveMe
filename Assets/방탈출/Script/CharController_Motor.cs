using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour
{
    #region 변수선언
    public float speed = 8.0f;
    public float sensitivity = 10.0f;

    public GameObject FpsController;
    public bool isSmall;
    CharacterController character;
    public GameObject cam;
    float moveFB, moveLR;
    float rotX, rotY;
    public bool webGLRightClickRotation = true;
    float gravity = -9.8f;

    public bool isPanelActive;  // UI Panel 뜨면 화면 고정
    public List<Animator> Anim;
    public GameObject etcAnim;
    public List<AudioSource> Audio;

    public bool[] isPlaying;
    public float animCoolTime;


    #endregion

    #region 싱글톤
    private static CharController_Motor Instance;

    public static CharController_Motor _Instance
    {
        get { return Instance; }
    }
    void Awake()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        isSmall = false;
        animCoolTime = 0f;

        // CameraRotation(cam, 0, 0);//시작시 카메라위치 고정(맞아??)

        character = GetComponent<CharacterController>();
        if (Application.isEditor)
        {
            webGLRightClickRotation = false;
            sensitivity = sensitivity * 1.5f;
        }
        isPanelActive = false;

        for (int i = 0; i < etcAnim.transform.childCount; i++)
        {
            isPlaying[i] = false;
            Anim.Add(etcAnim.transform.GetChild(i).GetComponent<Animator>());
            Audio.Add(etcAnim.transform.GetChild(i).GetComponent<AudioSource>());
        }
    }

    void Update()
    {
        //CharacterEvent();
        movement();
        StartCoroutine(CharacterEvent());
        for (int i = 0; i < etcAnim.transform.childCount; i++)
        {
            if (isPlaying[i])
            {
                animCoolTime += Time.deltaTime;
            }

            if (animCoolTime > 30f)  //애니메이션 쿨타임 20초
            {
                isPlaying[i] = false;
                animCoolTime = 0f;
            }
        }
    }

    #region 캐릭터움직임
    void movement()
    {
        moveFB = Input.GetAxis("Horizontal") * speed;
        moveLR = Input.GetAxis("Vertical") * speed;

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        //rotX = Input.GetKey (KeyCode.Joystick1Button4);
        //rotY = Input.GetKey (KeyCode.Joystick1Button5);

        Vector3 movement = new Vector3(moveFB, gravity, moveLR);

        //Ui패널 뜨면 화면 고정시키기
        if (!isPanelActive)
        {
            if (webGLRightClickRotation)
            {
                //if (Input.GetKey(KeyCode.Mouse0))
                //{
                CameraRotation(cam, rotX, rotY);
                //}
            }
            else if (!webGLRightClickRotation)
            {
                CameraRotation(cam, rotX, rotY);
            }
        }

        movement = transform.rotation * movement;
        character.Move(movement * Time.deltaTime);
    }
    #endregion

    #region 카메라위치

    void CameraRotation(GameObject cam, float rotX, float rotY)
    {
        transform.Rotate(0, rotX * Time.deltaTime * speed, 0);
        cam.transform.Rotate(-rotY * Time.deltaTime * speed, 0, 0);
    }
    #endregion


    #region Left Shift키로 캐릭터 앉기
    IEnumerator CharacterEvent()
    {
        yield return new WaitForSeconds(1f);
        //isSmall이 false이고 Z코드 누를 경우 키 작아지도록 설정(하단 힌트 보이도록)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSmall)
        {

            float smallY = transform.localScale.y - 0.7f;
            transform.localScale = new Vector3(transform.localScale.x, smallY, transform.localScale.z);
            isSmall = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            float OriginY = 1.5f;
            transform.localScale = new Vector3(transform.localScale.x, OriginY, transform.localScale.z);
            isSmall = false;
        }
    }
    #endregion

    //fps캐릭터와 흔들의자 박스콜라이더와 충돌했을경우 애니메이션과 사운드 Play
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "3f_floor_disable")
        {
            print("천장 무너짐 이벤트");
            Anim[1].SetBool("IsStopped", false);
        }

        else if (other.gameObject.name == "swingChair")
        {
            print("의자 흔들 이벤트");
            if (!isPlaying[0])
            {
                Anim[0].SetBool("IsStopped", false);
                Audio[0].PlayOneShot(Audio[0].clip);
                isPlaying[0] = true;
            }
        }
        else if (other.gameObject.name == "ChairSchool1")
        {
            print("의자 공격 이벤트1");
            if (!isPlaying[2])
            {
                Anim[2].SetBool("IsStopped", false);
                Audio[2].PlayOneShot(Audio[2].clip);
                isPlaying[2] = true;
            }
        }
        else if (other.gameObject.name == "ChairSchool2")
        {
            print("의자 공격 이벤트2");
            if (!isPlaying[3])
            {
                Anim[3].SetBool("IsStopped", false);
                Audio[3].PlayOneShot(Audio[3].clip);
                isPlaying[3] = true;
            }
        }
        else if (other.gameObject.name == "Chandelier")
        {
            print("샹들리에 공격 이벤트");
            if (!isPlaying[4])
            {
                Anim[4].SetBool("IsStopped", false);
                Audio[4].PlayOneShot(Audio[4].clip);
                isPlaying[4] = true;
                // animCoolTime -= Time.deltaTime;
            }

        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.gameObject.name == "Chandelier")
    //    {
    //        if (isPlaying)
    //        {
    //            print("샹들리에 공격 이벤트 STAY");
    //            Audio[4].
    //        }
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("chair"))
        {
            for (int i = 0; i < etcAnim.transform.childCount; i++)
            {
                print("트리거 exit");
                
            }
        }
    }
}




