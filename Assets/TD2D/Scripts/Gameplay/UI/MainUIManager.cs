using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class MainUIManager : MonoBehaviour
{
    [SerializeField] private int maxSpeed = 3;
    [SerializeField] private TextMeshProUGUI speedText;
    private int currSpeed = 1;
    public void OnClickBtnSpeedUp()
    {
        if (++currSpeed > maxSpeed)
            currSpeed = 1;
        Time.timeScale = currSpeed;
        speedText.text = String.Format("X{0}", currSpeed);
    }
}
