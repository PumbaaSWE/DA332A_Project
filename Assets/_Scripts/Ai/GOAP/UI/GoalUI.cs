using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoalUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] Slider Priority;
    [SerializeField] TextMeshProUGUI Status;



    public void UpdateGoalInfo(string name, string status, float priority)
    {
        Name.text = name;
        Status.text = status;
        Priority.value = priority;
    }
}