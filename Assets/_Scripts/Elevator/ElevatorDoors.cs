using System;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{

    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private float openTime = 1;
    [SerializeField] private float doorSpeed = 1;

    public event Action OnDoorsClosed;
    public event Action OnDoorsOpened;

    public bool Locked;

    public enum DoorStatus { Open, Opening, Closed, Closing }
    private DoorStatus status;
    public DoorStatus Status => status;

    Vector3 leftClosePos;
    Vector3 rightClosePos;

    Vector3 leftOpenPos;
    Vector3 rightOpenPos;
    float doorPercent;
    float timer;

    void Start()
    {
        ComputeDoorPos();
        doorPercent = 0;
        SetDoorPositions(doorPercent);
        status = DoorStatus.Closed;
        
    }

    public void ComputeDoorPos()
    {
        leftClosePos = leftDoor.localPosition;
        rightClosePos = rightDoor.localPosition;
        leftOpenPos = leftClosePos - Vector3.right;
        rightOpenPos = rightClosePos + Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        
        float dt = Time.deltaTime;
        switch (status)
        {
            case DoorStatus.Open:
                break;
            case DoorStatus.Opening:
                HandleOpening(dt);
                break;
            case DoorStatus.Closed:
                HandleClosed(dt);
                break;
            case DoorStatus.Closing:
                HandleClosing(dt);
                break;
            default:
                break;
        }
    }

    private void HandleClosed(float dt)
    {
        timer += dt;
        if(timer >= openTime)
        {
            OpenDoors();
            timer = 0;
        }
    }

    [MakeButton]
    public void CloseDoors()
    {
        status = DoorStatus.Closing;
       // ComputeDoorPos();
    }

    [MakeButton]
    public void OpenDoors()
    {
        if (Locked)
        {
            status = DoorStatus.Closed;
            return;
        }
        status = DoorStatus.Opening;
        //ComputeDoorPos();
    }

    private void HandleOpening(float dt)
    {
        //timer += dt;
        //float t = timer / openCloseTime;
        //SetDoorPositions(t);
        //if(t >= 1)
        //{
        //    status = DoorStatus.Open;
        //    OnDoorsOpened?.Invoke();
        //}

        doorPercent = Mathf.MoveTowards(doorPercent, 1, doorSpeed * dt);
        SetDoorPositions(doorPercent);
        if (doorPercent >= 1)
        {
            status = DoorStatus.Open;
            OnDoorsOpened?.Invoke();
        }
    }

    private void HandleClosing(float dt)
    {
        //timer += dt;
        //float t = timer / openCloseTime;
        //SetDoorPositions(1-t);
        //if (t <= 0)
        //{
        //    status = DoorStatus.Closed;
        //    OnDoorsClosed?.Invoke();
        //}
        doorPercent = Mathf.MoveTowards(doorPercent, 0, doorSpeed * dt);
        SetDoorPositions(doorPercent);
        if (doorPercent <= 0)
        {
            status = DoorStatus.Closed;
            OnDoorsClosed?.Invoke();
            timer = 0;
        }
    }

    private void SetDoorPositions(float t)
    {
        leftDoor.localPosition = Vector3.Lerp(leftClosePos, leftOpenPos, t);
        rightDoor.localPosition = Vector3.Lerp(rightClosePos, rightOpenPos, t);
    }
}
