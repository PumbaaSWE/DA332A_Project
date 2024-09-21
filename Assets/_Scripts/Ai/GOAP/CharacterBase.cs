using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterBase : MonoBehaviour
{

    //public int magasin = 4;
    //public int bulletsInMagasin = 7;
    //public bool noAmmo;
    [SerializeField] TextMeshProUGUI FeedbackLabel;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateHealthDisplay();

    }


    protected virtual void Update()
    {

    }
    protected virtual void Reseting()
    {
      
    }

    protected void UpdateHealthDisplay()
    {
        // FeedbackLabel.text = string.Format("{0} / {1}", currentHealth, maxHealth);
    }
    public Vector3 GetPosition()
    {
        return this.transform.position;
    }

    public void TakeDamage(int amount)
    {
      
        UpdateHealthDisplay();
    }

    public void RepairDamage(int amount)
    {
    
        UpdateHealthDisplay();
    }

   
}