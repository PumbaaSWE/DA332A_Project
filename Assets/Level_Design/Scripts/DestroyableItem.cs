using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyabaleItem : MonoBehaviour
{
    public GameObject cube;
    public GameObject debrisPrefab;
    public int health;
    private int damage;
    public void ChangeColor()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        //ChangeColor();
        if(health <= 0f)
        {
            Break();
        }
    }

    public void Break()
    {
        GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
        debris.transform.localScale = this.transform.localScale;
        Destroy(cube);
    }
}
