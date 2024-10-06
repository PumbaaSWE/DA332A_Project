using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NumPad : MonoBehaviour
{
    [SerializeField] private string code = "1234";
    [SerializeField] private TMP_Text text;
    private string input = "----";

    public UnityEvent OnCodeAccepted;

    public string Code { 
        get { return code; } 
        set { 
            if (value.Length == 4) 
            { 
                //also make sure its only numbers...
                code = value; 
            } 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        text.text = input;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressButton(char c)
    {
        input += c;
        if(input.Length == 4)
        {
            if (input.Equals(code))
            {
                OnCodeAccepted?.Invoke();
            }
        }
        if (input.Length > 4)
        {
            input = c.ToString();
        }
        text.text = input;
    }

    public void PressButton(int  i)
    {
        input += i.ToString();
        if (input.Length == 4)
        {
            if (input.Equals(code))
            {
                OnCodeAccepted?.Invoke();
            }
            else
            {
                
            }
        }
        if (input.Length > 4)
        {
            input = i.ToString();
        }
        //worst right pad ever-.-
        switch (input.Length)
        {
            case 0:
                text.text = "----";
                break;
            case 1:
                text.text = input + "---";
                break;
            case 2:
                text.text = input + "--";
                break;
            case 3:
                text.text = input + "-";
                break;
            case 4:
                text.text = input;
                break;
            default:
                text.text = "err.";
                break;
        }

        //text.text = input;
    }

    private string Pad(string s)
    {
        
        return s;
    }
}
