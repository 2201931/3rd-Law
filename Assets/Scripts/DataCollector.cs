using Abertay.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector
{
    public static DataCollector Instance { get; set; }

    public float matchTime = 0.0f;//
    public int WinnerPlayerID = -1;//

    public int P1_ShotsFired = 0;//
    public int P2_ShotsFired = 0;//

    public float P1_MaxRotationSpeed = 0;//
    public float P2_MaxRotationSpeed = 0;//

    public float P1_TotalTorqueApplied = 0;//
    public float P2_TotalTorqueApplied = 0;//

    public float P1_AverageSpeed = 0;//
    public float P2_AverageSpeed = 0;//

    public int P1_ButtonPressesPerShot = 0;
    public int P2_ButtonPressesPerShot = 0;


    public float P1_TotalSpeed = 0.0f;
    public float P2_TotalSpeed = 0.0f;

    public float P1_ButtonsPressed = 0.0f;
    public float P2_ButtonsPressed = 0.0f;



    public void Reset()
    {
        matchTime = 0.0f;
        WinnerPlayerID = -1;

        P1_ShotsFired = 0;
        P2_ShotsFired = 0;

        P1_MaxRotationSpeed = 0;
        P2_MaxRotationSpeed = 0;

        P1_TotalTorqueApplied = 0;
        P2_TotalTorqueApplied = 0;

        P1_AverageSpeed = 0;
        P2_AverageSpeed = 0;

        P1_TotalSpeed = 0.0f;
        P2_TotalSpeed = 0.0f;

        P1_ButtonsPressed = 0.0f;
        P2_ButtonsPressed = 0.0f;
    }

    public void Log(string eventID)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data["matchTime"] = Instance.matchTime;
        data["WinnerID"] = Instance.WinnerPlayerID;

        data["P1_ShotsFired"] = Instance.P1_ShotsFired;
        data["P2_ShotsFired"] = Instance.P2_ShotsFired;

        data["P1_MaxRotationSpeed"] = Instance.P1_MaxRotationSpeed;
        data["P2_MaxRotationSpeed"] = Instance.P2_MaxRotationSpeed;

        data["P1_TotalTorqueApplied"] = Instance.P1_TotalTorqueApplied;
        data["P2_TotalTorqueApplied"] = Instance.P2_TotalTorqueApplied;

        data["P1_AverageSpeed"] = Instance.P1_AverageSpeed;
        data["P2_AverageSpeed"] = Instance.P2_AverageSpeed;

        data["P1_ButtonPressesPerShot"] = Instance.P1_ButtonPressesPerShot;
        data["P2_ButtonPressesPerShot"] = Instance.P2_ButtonPressesPerShot;

        AnalyticsManager.SendCustomEvent(eventID, data);
    }
}
