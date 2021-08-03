using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Data;
using Mono.Data.Sqlite;       // db 사용
using UnityEngine.Networking; // 네트워크 사용
using Mono.Data.SqliteClient;

using System;
using UnityEngine.UI;
using SqliteConnection = Mono.Data.Sqlite.SqliteConnection;

public class sqlite : MonoBehaviour
{
    public static Text test; // DB 상태확인용 UI - Text

    // DBCreate()는 DB 생성하는 코드인데 우리는 굳이 사용할 필요 없음
    // IEnumerator 쓴 이유는 가장 빠르게 생성되기 때문
    // 실행하면 에셋 파일 안에 생성됨, db파일만 생성되기 때문에 테이블은 따로 만들어줘야 함
    IEnumerator DBCreate() // 거의 사용 안함
    {
        string filepath = string.Empty;
        if (Application.platform == RuntimePlatform.Android)
        {
            filepath = Application.persistentDataPath + "/test1.db"; //생성될 파일 경로와 db 이름
            if (!File.Exists(filepath))
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/test1.db"); //생성하고 싶은 경로와 db 이름
                unityWebRequest.downloadedBytes.ToString();
                yield return unityWebRequest.SendWebRequest().isDone;
                File.WriteAllBytes(filepath, unityWebRequest.downloadHandler.data);
            }
        }
        else
        {
            filepath = Application.dataPath + "/test1.db";
            if (!File.Exists(filepath))
            {
                File.Copy(Application.streamingAssetsPath + "/test1.db", filepath);
            }
        }
        Debug.Log("db생성 완료");
    }

    public static string GetDBFilePath() //파일 경로를 가져오는 코드 - 자주 사용함
    {
        string str = string.Empty;
        if (Application.platform == RuntimePlatform.Android)
        {
            str = "URI=file:" + Application.persistentDataPath + "/StreamingAssets/TT.db";
        }
        else
        {
            str = "URI=file:" + Application.dataPath + "/StreamingAssets/TT.db";
        }
        return str;
    }
    

    void Start()
    {
        DbConnectionCHek();
    }

    public static void DatabaseInsert(string query) // 삽입이라고 썼지만 삭제, 수정도 가능 // 필수 필수 !!!
    {
        IDbConnection dbConnection = new SqliteConnection(GetDBFilePath());
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();

        dbCommand.CommandText = query;
        dbCommand.ExecuteNonQuery();

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    public static void DataBaseRead(String query) // DB 읽어오기 // 필수 필수 !!!
    {
        IDbConnection dbConnection = new SqliteConnection(GetDBFilePath());
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = query;
        IDataReader dataReader = dbCommand.ExecuteReader();
        while (dataReader.Read())
        {
            Debug.Log(dataReader.GetString(0));
        }
        dataReader.Dispose();
        dataReader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    public static void DbConnectionCHek() // 연결상태 확인 // 필수~~
    {
        try
        {
            IDbConnection dbConnection = new SqliteConnection(GetDBFilePath());

            dbConnection.Open();

            if (dbConnection.State == ConnectionState.Open)
            {
                Debug.Log("db 연결 성공");
            }
            else
            {
                Debug.Log("db 연결 실패");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}