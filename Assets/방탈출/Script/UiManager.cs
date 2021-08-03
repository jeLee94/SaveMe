using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public GameObject StartPanel;
    public GameObject panel_parent;
    public float Playtime;
    public Text Playtimetxt;
    
    public List<GameObject> panels;

    #region 싱글톤
    private static UiManager Instance;

    public static UiManager _Instance
    {
        get { return Instance; }
    }
    #endregion
    void Awake()
    {
        Instance = this;
        //Ladder_check = true;
        //조작키 패널
        StartPanel = GameObject.Find("StartPanel");
    }
    // Start is called before the first frame update
    void Start()
    {
        Playtime = 0f;

        //코루틴으로 조작키 패널 깜빡깜빡
        StartCoroutine(PanelOnOff());

        for (int i = 0; i < panel_parent.transform.childCount; i++)
        {
            panels.Add(panel_parent.transform.GetChild(i).gameObject);
            panels[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Playtime += Time.deltaTime;
        var min = Playtime / 60;
        var sec = Playtime % 60;
        Playtimetxt.text = "Playtime : " + (int)min + "m " + (int)sec + "s";
    }

    //시작시 조작패널 깜빡임으로 알려주기
    IEnumerator PanelOnOff()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.75f);
            StartPanel.SetActive(false);
            yield return new WaitForSeconds(0.75f);
            StartPanel.SetActive(true);
        }
        yield return new WaitForSeconds(3f);
        StartPanel.SetActive(false);
    }

    //콜라이더 충돌 시 상호작용키 패널 켜주기
    #region 트리거 패널 이벤트
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            panels[0].SetActive(true);
            // Debug.Log("Door와 충돌함");
        }
        if (other.CompareTag("NP_Door"))
        {
            panels[0].SetActive(true);
            // Debug.Log("Door와 충돌함");
        }
        if (other.CompareTag("Hint"))
        {
            panels[0].SetActive(true);
            //  Debug.Log("Hint와 충돌함");
        }
        if (RayClickEvent._Instance.Hint[8].activeSelf == false && RayClickEvent._Instance.Ladder_check) //사다리 SetActive가 false일 때
        {
            if (other.gameObject.name == "3F_Door1")
            {
                panels[5].SetActive(true);
                
            }
            else if (other.gameObject.name == "3F_Door2")
            {
                panels[5].SetActive(true);
            }
        }
        
    }
    //충돌범위 벗어날 경우 패널 false
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            panels[0].SetActive(false);
            //   Debug.Log("Door와 떨어짐");
        }
        if (other.CompareTag("NP_Door"))
        {
            panels[0].SetActive(false);
            //   Debug.Log("Door와 떨어짐");
        }
        if (other.CompareTag("Hint"))
        {
            panels[0].SetActive(false);
            //  Debug.Log("Hint와 떨어짐");
        }
        if (other.CompareTag("Ladder") && RayClickEvent._Instance.Hint[8].activeSelf == false)
        {
            panels[5].SetActive(false);

        }
    }
    #endregion

    #region 메인으로 씬체인지
    public void goMain()
    {
        SceneManager.LoadScene("Main");
        Time.timeScale = 1f;
    }
    #endregion

}
