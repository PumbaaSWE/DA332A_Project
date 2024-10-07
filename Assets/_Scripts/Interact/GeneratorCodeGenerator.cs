using TMPro;
using UnityEngine;

public class GeneratorCodeGenerator : MonoBehaviour
{
    //temp
    public GameObject cieling;
    public NumPad numPad;
    public TMP_Text text;

    void Start()
    {
        if(cieling)cieling.SetActive(true);
        numPad.Code = Generate();
        text.text = numPad.Code;
    }

    private string Generate()
    {
        return Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString() + Random.Range(0, 10).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
