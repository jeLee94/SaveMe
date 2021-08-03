using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChageManager : MonoBehaviour
{

    public GameObject MainScenePannel;
    public GameObject SoundEffect;
    public GameObject BGM;
    public GameObject SettingPanel;
    public GameObject BGMmute;
    public GameObject SEmute;
    //public GameObject SE_Scroll;
    //public AudioSource SE_Vol;
    //public AudioSource BGM_Vol;
    //public GameObject BGM_Scroll;



    #region 싱글톤
    private static SceneChageManager instance;
    public static SceneChageManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<SceneChageManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Sound Class").AddComponent<SceneChageManager>();
                    instance = newSingleton;
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<SceneChageManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    #region 씬전환(메인->게임)
    public void goGame()
    {
        SceneManager.LoadScene("SaveMe");
        Time.timeScale = 1f;
        StartCoroutine(StartDelay(3));
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {

        SettingPanel.SetActive(false);
        SoundEffect.SetActive(false);
        BGMmute.SetActive(false);
        SEmute.SetActive(false);
        // FinalPanel.SetActive(false);
    }
    #region 시작시 화면안돌도록 1초딜레이
    IEnumerator StartDelay(int i)
    {
        while (i < 3)
        {
            yield return new WaitForSecondsRealtime(1f);
            i++;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
    #endregion
    // Update is called once per frame
    void Update()
    {



    }

}
