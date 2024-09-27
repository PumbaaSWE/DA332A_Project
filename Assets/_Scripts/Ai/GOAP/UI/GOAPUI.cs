using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPUI : MonoBehaviour
{
    [SerializeField] GameObject GoalPrefab;
    [SerializeField] RectTransform GoalRoot;

    Dictionary<MonoBehaviour, GoalUI> DisplayedGoals = new Dictionary<MonoBehaviour, GoalUI>();


    public void UpdateGoal(MonoBehaviour goal, string name, string status, float priority)
    {
        // add if not present
        if (!DisplayedGoals.ContainsKey(goal))
            DisplayedGoals[goal] = Instantiate(GoalPrefab, Vector3.zero, Quaternion.identity, GoalRoot).GetComponent<GoalUI>();

        DisplayedGoals[goal].UpdateGoalInfo(name, status, priority);
    }
}