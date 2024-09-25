
using UnityEngine;

public class MakeEnemyFollow : MonoBehaviour
{
    Transform toFollow;

    public MoveTowardsController[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toFollow && enemies != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetTarget(toFollow.position);
            }
        }
    }

    public void StartFollow(Transform transform)
    {
        toFollow = transform;
    }
}
