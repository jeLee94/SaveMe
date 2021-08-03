using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayClickEvent : MonoBehaviour
{
    #region 변수선언
    public RaycastHit Hit;
    public bool isHit;
    public float MaxDistance = 0.3f;

    public GameObject Spotlight;
    public GameObject FilmProjector;  //F키 누를 경우 프로젝터에 힌트 보여주기
    public GameObject F3Door1; // 3층가는 사다리 위치1
    public GameObject F3Door2; // 3층가는 사다리 위치2

    //문 변수
    public GameObject DoorManager;
    public GameObject NP_DoorManager;
    public List<Animator> Anit;
    public List<AudioSource> AudioList;//비밀번호맞을때 문열림소리 넣어줄 오디오소스 리스트, 문리스트랑 똑같이 for문 돌리기
    public int doornum;  //문배열변수
    public int np_doornum; //비밀번호필요없는 문 배열 변수

    //힌트 변수
    public GameObject HintManager;
    public GameObject[] Hint;
    public int counthint;

    // 키패드 패널
    public Text password;
    public string password_text;
    public bool Ladder_check; //사다리 한쪽만 설치를 위한 체크용 불변수


    //UiManager에서 싱글톤으로 패널리스트 가져오기위한 변수 선언
    public List<GameObject> panels;
    //패스워드체크할 문 리스트
    public List<string> password_door;

    //슬롯클릭시 띄워줄 큰 이미지
    public Image SlotImage;
    

    
    #endregion

    #region 싱글톤
    private static RayClickEvent Instance;

    public static RayClickEvent _Instance
    {
        get { return Instance; }
    }
    void Awake()
    {
        Instance = this;
    }
    #endregion

    public static string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
    void Start()
    {

        
        isHit = false;
        Ladder_check = true;

        //애니메이터 리스트에 도어매니저 자식 컴포넌트 순서대로 넣어주기
        for (int i = 0; i < DoorManager.transform.childCount; i++)
        {
            Anit.Add(DoorManager.transform.GetChild(i).GetComponent<Animator>());
            AudioList.Add(DoorManager.transform.GetChild(i).GetComponent<AudioSource>());
        }
        for (int i = 0; i < NP_DoorManager.transform.childCount; i++)
        {
            Anit.Add(NP_DoorManager.transform.GetChild(i).GetComponent<Animator>());
            AudioList.Add(NP_DoorManager.transform.GetChild(i).GetComponent<AudioSource>());
        }

        doornum = 0;
        counthint = 0;

        //Hint배열에 힌트매니저 자식 컴포넌트 순서대로 넣어주기
        Hint = new GameObject[HintManager.transform.childCount + 1];
        int j = 0;
        for (; j < HintManager.transform.childCount + 1; j++)
        {
            if (j < 2)
                Hint[j] = HintManager.transform.GetChild(j).gameObject; // Error

            else if (j == 2)
                Hint[j] = GameObject.Find("Hint2");

            else if (j > 2)
                Hint[j] = HintManager.transform.GetChild(j - 1).gameObject; // Error
        }


        //반복문으로 리스트에 공백채워주기(비밀번호 필요없는 문은 제외하고 Insert를 하기 위함) //비밀번호 입력 네자리로 제한할지 고민중
        for (int i = 0; i < DoorManager.transform.childCount; i++)
            password_door.Add("");
        password_door.Insert(0, "1256");
        password_door.Insert(1, "4652");
        password_door.Insert(2, "7638");
        password_door.Insert(3, "2515");
        password_door.Insert(4, "1467");
        //마지막 비밀번호 난수값으로 바꿔보기

        //싱글톤으로 패널가져오기
        panels = UiManager._Instance.panels;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //레이캐스트 코드 호출
        RaycastCode();
        if (isHit)
        {
            DoorEvent();
            HintEvent();
            InventoryEvent();
        }
        var hint = "";
        var door = "";
        for (int i = 0; i < Hint.Length; i++)
        {

            if (Hit.transform.gameObject.name == Hint[i].name)
                hint = Hint[i].name;
           // break;
        }
        for (int i = 0; i < Anit.Count; i++)
        {
            if (Hit.transform.gameObject.name == Anit[i].name)
                door = Anit[i].name;
           // break;
        }
        SaveMe_Data temp = new SaveMe_Data
        {
            playtime = UiManager._Instance.Playtime,
            Hit_Hint = hint,
            Hit_Door = door

        };
        if (hint != "" || door != "")
            CSVData.Data.Add(temp);
    }

    #region 레이캐스트
    void RaycastCode()
    {
        // 씬창에서 레이 그려주는 코드
        Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.yellow, 0.3f);
        if (Physics.Raycast(transform.position, transform.forward, out Hit, MaxDistance))
        {
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.red, 0.3f);
            isHit = true;
        }
        // 마우스 클릭한 곳을 월드좌표로 변환해주는 코드


        var Point = Camera.main.ScreenToWorldPoint
        (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

    }
    #endregion


    #region 화면고정
    //패널 액티브 시 화면고정함수
    public void ScreenEnable(bool state)
    {
        CharController_Motor._Instance.isPanelActive = state;
    }
    #endregion


    #region 문이벤트(F키)
    void DoorEvent()
    {
        //문 이벤트
        for (int i = 0; i < Anit.Count; i++)
        {
            // print("1. "+Hit.transform.gameObject.name);
            // print("2. " + Anit[i].name);
            if (Hit.transform.gameObject.name == Anit[i].name)
                doornum = i;
        }
        for (int j = 0; j < Anit.Count; j++)
        {
            if (Hit.transform.gameObject.name == Anit[j].name)
                np_doornum = j;
        }


        if (Input.GetKey(KeyCode.F)) // 키보드에서 F누를 경우 (GetKey로 하면 문이 여러번 열렸다 닫히고(애니메이터 트랜지션 HasExitTime 체크하면 괜춘한가?, GetKeyDown으로 하면 인식이 잘안됨)
        {
            //문이 닫혀있는 상태면
            if (Anit[doornum].GetBool("IsClosed"))
            {
                //비밀번호가 필요없는문 예외처리
                if (Hit.transform.gameObject.tag == "NP_Door")
                {
                    Anit[doornum].SetBool("IsClosed", false);
                    AudioList[doornum].PlayOneShot(AudioList[doornum].clip);
                }


                //문을 열어준다.
                if (Hit.transform.gameObject.tag == "Door")
                {
                    //비밀번호 입력 패널 띄우기
                    panels[1].SetActive(true);
                    //화면 고정(패널에 집중)
                    ScreenEnable(true);
                    //커서가 게임화면에 보이도록 설정
                    Cursor.visible = true;
                    //커서 고정 해제
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            //문이 열려있는 상태면
            else
            {
                //비밀번호가 필요없는문 예외처리
                if (Hit.transform.gameObject.tag == "NP_Door")
                {
                    Anit[doornum].SetBool("IsClosed", true);
                    AudioList[doornum].PlayOneShot(AudioList[doornum].clip);

                }
                //문을 닫아준다.
                if (Hit.transform.gameObject.tag == "Door")
                {

                    Anit[doornum].SetBool("IsClosed", true);
                    AudioList[doornum].PlayOneShot(AudioList[doornum].clip);
                }
            }
        }
        isHit = false;
    }
    #endregion

    #region 힌트획득이벤트(F키)
    void HintEvent()
    {

        //힌트리스트에 힌트 목록 넣어주기
        for (int i = 0; i < Hint.Length; i++)
        {
            if (Hit.collider.gameObject.name == Hint[i].name)
            {
                counthint = i;
                break;
            }
        }

        if (Input.GetKey(KeyCode.F)) // 키보드에서 F누를 경우
        {
            //print("1. " + Hit.collider.gameObject.name);
            //print("2. " + Hint[counthint].name);
            //print("3. " + counthint);
            //print(Hit.transform.gameObject.name);
            // 인벤토리에 아이템 들어가고 해당 오브젝트 setactive(false);

            if (Hit.collider.gameObject.name == "FilmProjector")
            {
                var three = FilmProjector.transform.GetChild(0);
                var spotlight2 = FilmProjector.transform.GetChild(1);
                three.gameObject.SetActive(true);
                spotlight2.gameObject.SetActive(true);
                AudioList[25].PlayOneShot(AudioList[25].clip);

            }


            if (Hit.collider.gameObject.name == "Item_Flashlight")
            {
                Spotlight.SetActive(true);
                Inventory._Instance.GetItem("Item_Flashlight");
                Hint[counthint].SetActive(false);
                panels[0].SetActive(false);
                //break;
            }

            //else if (Hit.collider.gameObject.name == "3F_Door1")
            //{
            //    var Radder1 = F3Door1.transform.GetChild(0);
            //    Radder1.gameObject.SetActive(true);
            //    break;
            //}
            //if (Hint[counthint].name == "Item_Ladder")
            //{
            //    if (Hit.collider.gameObject.name == "3F_Door1")
            //    {
            //        var Radder1 = F3Door1.transform.GetChild(0);
            //        Radder1.gameObject.SetActive(true);
            //        break;
            //    }
            //}


            //for (int i = 1; i < Hint.Length; i++)
            //{

            else if (Hit.collider.gameObject.name == Hint[counthint].name)
            {
                Inventory._Instance.GetItem(Hint[counthint].name);
                Hint[counthint].SetActive(false);
                panels[0].SetActive(false);
                //break;
            }

            //사다리 먹었을 경우에만 사다리 위치에서 활성화 시킬수있음
            //양쪽다 설치안되도록 불변수 사용
            if (Hint[8].activeSelf == false && Ladder_check == true)
            {
                var Ladder1 = F3Door1.transform.GetChild(0);
                var Ladder2 = F3Door2.transform.GetChild(0);
                if (Hit.collider.gameObject.name == "3F_Door1")
                {
                    Ladder1.gameObject.SetActive(true);
                    Ladder_check = false;
                }
                else if (Hit.collider.gameObject.name == "3F_Door2")
                {
                    Ladder2.gameObject.SetActive(true);
                    Ladder_check = false;
                }
                //if (Ladder_check == false)
                //{
                //    Ladder1.gameObject.SetActive(false);
                //    Ladder2.gameObject.SetActive(false);
                //}
            }


        }
    }

    #endregion

    #region 비밀번호입력
    public void Numpad(GameObject gameObject)
    {
        password.text += gameObject.name;

    }
    #endregion

    #region 패스워드 한개씩 지우기
    public void Backpassword()
    {
        if (password.text == "")
            return;
        password.text = password.text.Substring(0, password.text.Length - 1);
    }
    #endregion

    #region 비밀번호체크
    public void Answer_check()
    {
        print("Answer_check()");
        if (password.text == password_door[doornum])
        {
            //패널안보이게하기
            panels[1].SetActive(false);
            //화면 고정 해제
            ScreenEnable(false);
            AudioList[doornum].PlayOneShot(AudioList[doornum].clip);


            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
            Anit[doornum].SetBool("IsClosed", false);
            //패스워드 초기화는 버튼 클릭 인스펙터에서 대체
            if (doornum == 4 && Anit[doornum].GetBool(0) == false)
            {
                //애니메이션 진행시간 코루틴으로 3초 딜레이
                StartCoroutine(playDelay());
                
            }
        }
    }
    #endregion

    #region 최종 문 코루틴 3초딜레이
    IEnumerator playDelay()
    {
        yield return new WaitForSeconds(3f);

        panels[4].SetActive(true);
        ScreenEnable(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        
    }
    #endregion

    #region 인벤토리 이벤트(Z키)
    void InventoryEvent()
    {
        if (panels[2].activeSelf == false && Input.GetKeyDown(KeyCode.Z)) // 키보드에서 Z키누를 경우
        {

            //인벤토리 패널 띄우기
            panels[2].SetActive(true);
            //화면 고정(패널에 집중)
            ScreenEnable(true);
            //커서가 게임화면에 보이도록 설정
            Cursor.visible = true;
            //커서 고정 해제
            Cursor.lockState = CursorLockMode.None;
        }
        else if (panels[2].activeSelf == true && Input.GetKeyDown(KeyCode.Z))
        {
            //게임화면으로 돌아가기
            panels[2].SetActive(false);
            //화면 고정 해제(패널에 집중)
            ScreenEnable(false);
            //커서가 게임화면에 보이지 않도록 설정
            Cursor.visible = false;
            //커서 고정 
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panels[1].activeSelf == true)
            {
                panels[1].SetActive(false);
                password.text = "";
            }
            panels[2].SetActive(false);
            panels[3].SetActive(false);
            ScreenEnable(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion


    #region 힌트이미지 확대
    public void Idx(int idx) // 슬록 확장시 Idx넘버 체크해야함(안그러면 이미지중복됨)
    {
        var SI = Inventory._Instance.items[idx];

        if (SI.itemName.Substring(0, 4) == "Hint" || SI.itemName.Substring(0, 4) == "Item")
        {
            SlotImage.sprite = SI.itemImage;
            panels[3].SetActive(true);
            SlotImage.gameObject.SetActive(true);
        }
    }
    #endregion
}













