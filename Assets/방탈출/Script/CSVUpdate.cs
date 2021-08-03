using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ReadLineFile 빼고는 거의 다 필수적으로 사용함
public class CSVUpdate : MonoBehaviour
{
    public string date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");

    private static CSVUpdate instance;        // 클래스 내부에서만 접근가능한 클래스 타입의 멤버 변수 생성

    public static string SqlFormat(string sql)
    {
        return string.Format("\"{0}\"", sql);
    }
    // 주로 사용
    private void Awake()                      // 생명주기 Awake 
    {
        date = SqlFormat(date);
        if (instance != null)                 // instance 변수의 값이 null이 아닐경우
        {
            Destroy(instance);                // instance의 값을 파괴한다.
        }
        instance = this;                      // instance에 자기 자신 CSVUpdate 클래스 객체를 저장한다.
    }

    // 주로 사용
    public static CSVUpdate GetInstance()     // 외부에서 접근가능한 CSVUpdate 클래스 타입의 반환값을 가진 GetInstance 메소드 선언
    {
        if (instance == null)                 // instance값이 null이면
        {
            Debug.LogError("CSV 인스턴스가 존재하지 않습니다.");  //  로그에러 출력
            return null;                                          // null값을 반환한다.
        }
        // instance가 null이 아니면 instance 값을 반환한다.
        return instance;
    }

    // 외부에서 접근 가능한 ReadLinFile 메소드 선언, 매개변수로 문자열변수 file을 받는다.
    public void ReadLineFile(string file) // 아주 가끔 사용
    {
        if (System.IO.File.Exists(file))   // file이란 이름의 파일이 있는 지 확인, 있다면 
        {
            //List<Dictionary<string, object>> data = CSVReader.Read(file);       // data라는 딕셔너리 타입의 리스트에 CSVReader 스크립트의 Read 메소드 사용하여 파일의 내용을 읽어와 데이터를 저장한다.

            //for (var i = 0; i < data.Count; i++)                                // data리스트의 개수만큼 반복문 반복
            //{
            //    IMU_Sensor_Data Data = new IMU_Sensor_Data();                   // IMU_Sensor_Data(CSVData스크립트에서 새롭게 선언해준) 클래스 인스턴스 생성
            //                                                                    // **인스턴스란? 칸튼 클래스에 속하는 개개의 객체로, 하나의 클래스에서 생성된 객체를 말함
            //                                                                    //이 때 추상적인 개념의 클래스에서 실제 객체를 생성하는 것을 인스턴스화 라고 한다.
            //    Data.Accelerometer = Device_Data_Load.instance.Accelerometer;
            //    Data.Input_gyro_attitude_eulerAngles = Device_Data_Load.instance.Input_gyro_attitude_eulerAngles;
            //    Data.Input_gyro_rotationRateUnbiased = Device_Data_Load.instance.Input_gyro_rotationRateUnbiased;
            //    Data.playtime = Device_Data_Load.instance.playtime;
            //    CSVData.IMU_Data.Add(Data);                                     //  CSVData스크립트의 IMU_Sensor_Data 클래스 리스트 변수인 IMU_Data에 Data값 삽입
            //}
        }
    }

    // 파일 내용을 업데이트 하는 메소드 선언 
    public void UpdateLineFile() // 주로 사용
    {
        string filePath = CSVData.getPath();                          // CSVData의 getPath()메소드 사용하여 파일 경로를 문자열 변수 filePath에 저장
        StreamWriter outStream = System.IO.File.CreateText(filePath); // 파일에 내용을 쓰는 메소드를 outStream이름으로 선언 위에서 지정해준 filePath 문자열을 이용해 새로운 파일을 만들거나
                                                                      // filePath에 저장된 경로에 해당 파일명이 있다면 그 파일을 열어줌
        string str;                                                   // str 문자열 변수 선언

        // 
        outStream.WriteLine("PlayTime," +
                            "Hit_Hint," +
                            "Hit_Door");

        for(int i = 0; i<CSVData.Data.Count; i++)
        {
            str = CSVData.Data[i].playtime.ToString("N1") + ","
                + CSVData.Data[i].Hit_Hint + ","
                + CSVData.Data[i].Hit_Door;
            outStream.WriteLine(str);
        }
        // 
        //for (int i = 0; i < CSVData.IMU_Data.Count; i++)
        //{
        //    str = CSVData.IMU_Data[i].playtime.ToString("N1") + ","
        //        + CSVData.IMU_Data[i].Accelerometer.x + ","
        //        + CSVData.IMU_Data[i].Accelerometer.y + ","
        //        + CSVData.IMU_Data[i].Accelerometer.z + ","
        //        + CSVData.IMU_Data[i].Input_gyro_attitude_eulerAngles.x + ","
        //        + CSVData.IMU_Data[i].Input_gyro_attitude_eulerAngles.y + ","
        //        + CSVData.IMU_Data[i].Input_gyro_attitude_eulerAngles.z + ","
        //        + CSVData.IMU_Data[i].Input_gyro_rotationRateUnbiased.x + ","
        //        + CSVData.IMU_Data[i].Input_gyro_rotationRateUnbiased.y + ","
        //        + CSVData.IMU_Data[i].Input_gyro_rotationRateUnbiased.z;
        //    outStream.WriteLine(str); // str 변수에 저장된 문자열을 outStream 변수에 의해 생성된 파일에 입력한다.
        //}
        outStream.Close();            // 스트림을 종료. 종료하지 않으면 파일에 제대로 입력이 되지 않을 수 있다.
    }

    //종료됐을때 csv에 뿌려주는 메소드
    public void OnApplicationQuit() // 주로 사용
    {
            UpdateLineFile();         //파일 내용 업데이트하는 메소드 호출
        // DB 연동 코드
        sqlite.DbConnectionCHek(); // DB 연결, 연결상태 확인
        string sql = string.Format("Insert into Game(Datetime, Playtime, Score) VALUES({0}, {1}, {2})", date, UiManager._Instance.Playtime, 0);
        print(sql);
        sqlite.DatabaseInsert(sql); // 위에서 짠 SQL문을 디비에 쏴주는 함수 실행
    }
}
