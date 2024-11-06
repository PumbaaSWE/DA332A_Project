using TMPro;
using UnityEngine;

public class GeneratorCodeGenerator : MonoBehaviour
{
    //temp
    public GameObject cieling;
    public NumPad numPad;
    public TMP_Text text;
    public TMP_Text text2;

    void Start()
    {
        if(cieling)cieling.SetActive(true);
        numPad.Code = Generate();
        text.text = numPad.Code.Substring(0, 2);
        text2.text = numPad.Code.Substring(2, 2);
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
