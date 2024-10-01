using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public class FollowPlayer : MonoBehaviour, IInteractable
{

    bool isFollowing;

    MoveTowardsController[] controllers;

    public string tooltip = "Press {0} button!";
    public string interactedTooltip = string.Empty;
    public string Tooltip => tooltip;
    public string InteractedTooltip => interactedTooltip;
    public bool CanInteract => true;
    public bool ShowInteractMessage { get { return interactedTooltip != string.Empty; } }
    public int InteractedDisplayPriority => 0;
    public float InteractedTipDisplayTime => 0;

    Transform toFollow;

    public void Interact(Transform interactor)
    {
        isFollowing = !isFollowing;
        toFollow = interactor;
        UpdateTooltipText();
        if (!isFollowing)
        {
            foreach (var c in controllers)
            {
                c.SetTarget(((Random.onUnitSphere * 10) + transform.position).WithY());
            }
        }
    }

    private void UpdateTooltipText()
    {
        if (isFollowing)
        {
            tooltip = "Press {0} to make them stop following!";
        }
        else
        {
            tooltip = "Press {0} to make them follow!";
        }
    }

    void Start()
    {
        controllers = FindObjectsByType<MoveTowardsController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        UpdateTooltipText();
    }

    public void SetTarget(Vector3 point)
    {
        foreach (var c in controllers)
        {
            c.SetTarget(point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing)
        {
            SetTarget(toFollow.position);
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        //noop
    }
}
