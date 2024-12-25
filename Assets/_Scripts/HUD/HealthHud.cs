using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthHud : AbstractHUD<Health>
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image progressBar;
    [SerializeField] Gradient gradient;
    //float maxHealth;

    Health health;

    // Start is called before the first frame update
    void Start()
    {
        //only get the progressbar this way if not assigne as I dont like this way
        if(!progressBar) progressBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!health) {
            enabled = false;
            return; 
        }
        float percentage = health.Value / health.MaxHealth;

        percentage = Mathf.Clamp01(percentage);
        progressBar.color = gradient.Evaluate(percentage);

        progressBar.fillAmount = percentage; // apply progress first, calculate color after
        //percentage -= 0.5f * (1 - (percentage * 2 - 1)); // when at half health, percentage = 0. When at full health, percentage = 1. it is yes WHAT IS IT AFTER THIS MATH??
        //percentage = Mathf.Clamp(percentage, 0.0f, 1.0f);
        //float R = 1.0f;
        //float G = 1.0f;
        //if (percentage >= 0.5f)
        //{
        //    R = 1 * (1 - (percentage * 2 - 1));
        //}
        //if(percentage <= 0.5f)
        //{
        //    G = 2 * percentage;
        //}
        //R -= offset;
        //G -= offset;
        //R = Mathf.Clamp(R, 0.0f, 1.0f);
        //G = Mathf.Clamp(G, 0.0f, 1.0f);
        //progressBar.color = new Color(R, G, 0.0f, 1.0f);
    }


    protected override void SetScript(Health script)
    {
        health = script;
        enabled = true;
    }

    protected override void PlayerSet()
    {
        
    }

    protected override void PlayerRemoved()
    {
        health = null;
        enabled = false;
    }
}
