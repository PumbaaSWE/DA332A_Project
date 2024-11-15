using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamageHUD : MonoBehaviour
{
    public PlayerDataSO player;
    float tot;
    [SerializeField] float fadeTime = 0.4f;
    [SerializeField] float startFade = 0.8f;
    float t;
    public RawImage image;
    Color color;
    [SerializeField] Color healColor = Color.green;
    [SerializeField] Color damageColor = Color.red;

    Transform playerTransform;
    PlayerHitbox playerHitbox;
    [SerializeField] float directionalIndicatorTime = 0.6f;
    [SerializeField] DirectionalIndicator indicatorTemplate;
    readonly Queue<DirectionalIndicator> indicatorQ = new();
    //PriorityList<>
    readonly Stack<DirectionalIndicator> indicatorUnsusedStack = new();

    // Start is called before the first frame update
    void Start()
    {
        if (image == null)
        {
            image = GetComponent<RawImage>();
        }
        indicatorTemplate.Deactivate();
        //indicatorUnsusedStack.Push(indicatorTemplate);
        color = image.color;
    }

    private void OnEnable()
    {
        player.NotifyOnPlayerChanged(SetPlayer);
    }

    private void SetPlayer(Transform obj)
    {
        if (obj)
        {
            if (obj.TryGetComponent(out playerHitbox))
            {
                playerHitbox.OnHit += TakeDamage;
            }
            playerTransform = obj;
        }
    }

    private void OnDisable()
    {
        if (playerHitbox)
        {
            playerHitbox.OnHit -= TakeDamage;
        }
        player.UnsubscribeOnPlayerChanged(SetPlayer);
    }

    private void OnHealthChanged(float delta)
    {

    }

    private void TakeDamage(Vector3 point, Vector3 direction, float delta)
    {
        //image.material.color = UnityEngine.Color.red;
        //Debug.Log("HUD takes notice of damage");
        enabled = true;
        tot = t = fadeTime;
        if (delta > 0)
        {
            color = damageColor;
            DirectionalInicator(-direction, delta);
        }
        else
        {
            color = healColor;
        }
    }

    public void DirectionalInicator(Vector3 direction, float delta)
    {
        GetIndicator().Activate(direction, delta);
    }

    private DirectionalIndicator GetIndicator()
    {
        DirectionalIndicator indicator;
        if (indicatorUnsusedStack.Count == 0)
        {
            indicator = Instantiate(indicatorTemplate, transform);
            indicator.Init(this, playerTransform);
        }
        else
        {
            indicator = indicatorUnsusedStack.Pop();
        }
        indicatorQ.Enqueue(indicator);
        return indicator;
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;

        if (t <= 0)
        {
            image.color = new Color(color.r, color.g, color.b, 0);
        }

        image.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0, startFade, t / tot));

        //since all indicator live the same amount of time we only need to check the first in queue, the oldest.
        while(indicatorQ.Count > 0 && Time.time - indicatorQ.Peek().TimeCreated > directionalIndicatorTime)
        {
            indicatorUnsusedStack.Push(indicatorQ.Dequeue().Deactivate());
        }
    }
}