using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    Material material;
    Renderer rnd;
    void Start()
    {
        rnd = GetComponent<Renderer>();
        CreateTexture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void  CreateTexture()
    {
        var texture = new Texture2D(40, 40);
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                texture.SetPixel(i, j, Color.red);
            }
        }
        texture.Apply();

        rnd.material.mainTexture = texture;
    }


}
