using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ElevatorDoors))]
public class Elevator : MonoBehaviour
{

    private ElevatorDoors doors;
    private ElevatorLightEffect effect;
    [SerializeField] private BoxCollider insideTrigger;
    [SerializeField] private int nextSceneGroup = 1;
    [SerializeField] private GameObject insideButton;

    Transform player;
    bool loading;
    float time;

    public List<GameObject> gameObjectsToMove = new();

    void Start()
    {
        doors = GetComponent<ElevatorDoors>();
        doors.OnDoorsClosed += Doors_OnDoorsClosed;
        effect = GetComponent<ElevatorLightEffect>();
    }

    

    public bool CheckPlayerInside()
    {
        Collider[] colliders = Physics.OverlapBox(insideTrigger.center + insideTrigger.transform.position, insideTrigger.size / 2, transform.rotation);
        bool playerFound = false;
        gameObjectsToMove.Clear();
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                playerFound = true;
                gameObjectsToMove.Add(collider.transform.root.gameObject);
                //Debug.Log("Elevator - Playr found inside elevator");
            }
            //do more checks here
            //gameObjectsToMove.Add(collider.transform.root.gameObject);
        }
        if (!playerFound && player)
        {

            if(MathUtility.InsideBox(player.position, insideTrigger.center + insideTrigger.transform.position, insideTrigger.size / 2, transform.rotation))
            {
                gameObjectsToMove.Add(player.gameObject);
                playerFound = true;
            }


            
        }

        return playerFound;
    }

    public void PressInsideButton(Transform player)
    {

        if (player.CompareTag("Player"))
        {
            this.player = player.root;
        }
        doors.CloseDoors();
    }

    public void PressOutsideButton(Transform _)
    {
        //check to see if open or something to do something cool with the button...
        doors.OpenDoors();
    }


    private void Doors_OnDoorsClosed()
    {
        if (CheckPlayerInside())
        {
            Scene scene = gameObject.scene;
            for (int i = 0; i < gameObjectsToMove.Count; i++)
            {

                SceneManager.MoveGameObjectToScene(gameObjectsToMove[i], scene);
                
                
            }
        }
        //else
        //{
        //    doors.OpenDoors();
        //}
        //load next scene!!!
        effect.StartDown(6);
        doors.Locked = true;
        loading = true;
        SceneGroupLoader.Instance.OnLoadingComplete += Instance_OnLoadingComplete;
        SceneGroupLoader.Instance.LoadGroup(nextSceneGroup);
        time = Time.time;
        insideButton.SetActive(false); // replave with in active button?
        StartCoroutine(WaitUntilLoadCompletes());
    }

    private IEnumerator WaitUntilLoadCompletes()
    {
        yield return new WaitUntil(()=>!loading && Time.time - time > 6);
        doors.Locked = false;
        doors.OpenDoors();
    }

    private void OnDisable()
    {
        //can I not unsub here?
       
    }

    private void Instance_OnLoadingComplete()
    {
        loading = false;
        SceneGroupLoader.Instance.OnLoadingComplete -= Instance_OnLoadingComplete;
        //doors.Locked = false;
    }

    void Update()
    {
    }
}
