using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전체 다 주로 사용
public class CSVData : MonoBehaviour  
{
    // 새로만든 IMU_Sensor_Data 클래스의 리스트 (인스턴스) 생성
    public static List<SaveMe_Data> Data = new List<SaveMe_Data>();

    // 문자열을 반환할 getPath 메소드 선언
    public static string getPath()
    {
        // Application.dataPath는 프로젝트디렉토리/Assets폴더를 접근하는 속성이며 
        //Aseets 폴더 내에 /CSV라는 폴더를 생성하여 현재시간을 문자열로 나타내는 .csv파일을 생성 및 저장하는 코드
        return Application.dataPath + "/CSV/" + "Saved_data_" + System.DateTime.Now.ToString("yyyy년MM월dd일_HH시mm분ss초") + ".csv";
    }
}

// IMU_Sensor_Data의 이름으로 새로운 클래스 선언
public class SaveMe_Data
{
    public float playtime { get; set; } // 플레이타임
    public string Hit_Hint { get; set; } // Hit한 힌트
    public string Hit_Door { get; set; } // Hit한 문
    //public Vector3 Accelerometer { get; set; } // 가속도 x, y, z값
    //public Vector3 Input_gyro_attitude_eulerAngles { get; set; } // 자이로_angle x, y, z값
    //public Vector3 Input_gyro_rotationRateUnbiased { get; set; } // 자이로_speed x, y, z값
    
}
 