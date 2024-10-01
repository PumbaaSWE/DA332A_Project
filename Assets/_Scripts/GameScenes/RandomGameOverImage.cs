using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomGameOverImage : MonoBehaviour
{

    public Image gameOverImage;


    public Sprite[] gameOverImages;

    private void Start()
    {
      
        int randomIndex = Random.Range(0, gameOverImages.Length);
        gameOverImage.sprite = gameOverImages[randomIndex];
    }
}
