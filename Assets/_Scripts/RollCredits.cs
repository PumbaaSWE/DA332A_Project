using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RollCredits : MonoBehaviour
{
    [SerializeField] float fadeTime = 2.5f;
    [SerializeField] float creditFadeInTime = 1f;
    [SerializeField] float creditDisplayTime = 5f;
    [SerializeField] float creditFadeOutTime = 1f;

    [SerializeField] RawImage image;

    [SerializeField]
    [Tooltip("Texts to display in order of the list")]
    TMP_Text[] credits;
    //If we need individual times for each text tell meeee (or uncomment creditDatas and comment the credits... but not really tested)
    //[SerializeField]
    //CreditData[] creditDatas;


    async void Start()
    {
        //init, disable all other soundsListeners and cameras??




        if (credits != null)
        {
            for (int i = 0; i < credits.Length; i++)
            {
                credits[i].gameObject.SetActive(false);
            }
        }

        //if (creditDatas != null)
        //{
        //    for (int i = 0; i < creditDatas.Length; i++)
        //    {
        //        creditDatas[i].credit.gameObject.SetActive(false);
        //    }
        //}

        await FadeToBlack();


        //we have faded to black
        //we can unload all scenes except this but then need to enable cameras
        //issue atm is that previous level keep playing, sounds and what knot...
        //we could find all loaded scenes and unload all except this one


        if(credits != null)
        {

            for (int i = 0; i < credits.Length; i++)
            {
                await CreditFade(credits[i]);
            }
        }

        //if (creditDatas != null)
        //{
        //    for (int i = 0; i < creditDatas.Length; i++)
        //    {
        //        await CreditFade(creditDatas[i]);
        //    }
        //}

        //Debug.Log(" Completed dfhgsdfgh");

        //load menu
        //do some kind of error check here maybe if something happens?? 
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    async Task FadeToBlack()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeTime;
            image.color = Color.Lerp(Color.clear, Color.black, t);
            await Task.Yield();
        }
    }

    async Task CreditFade(TMP_Text credit)
    {

        credit.gameObject.SetActive(true);
        credit.color = Color.clear;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / creditFadeInTime;
            credit.color = Color.Lerp(Color.clear, Color.white, t);
            await Task.Yield();
        }

        await Task.Delay((int)(creditDisplayTime * 1000));

        while (t > 0)
        {
            t -= Time.deltaTime / creditFadeOutTime;
            credit.color = Color.Lerp(Color.clear, Color.white, t);
            await Task.Yield();
        }
        credit.gameObject.SetActive(false);
    }

    async Task CreditFade(CreditData credit)
    {

        credit.credit.gameObject.SetActive(true);
        credit.credit.color = Color.clear;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / credit.creditFadeInTime;
            credit.credit.color = Color.Lerp(Color.clear, Color.white, t);
            await Task.Yield();
        }

        await Task.Delay((int)(credit.creditDisplayTime * 1000));

        while (t > 0)
        {
            t -= Time.deltaTime / credit.creditFadeOutTime;
            credit.credit.color = Color.Lerp(Color.clear, Color.white, t);
            await Task.Yield();
        }
        credit.credit.gameObject.SetActive(false);
    }

}



//not used yet, maybe save the text string if all should be formated the same, or use rich-text or something
[Serializable]
public struct CreditData
{
    public float creditFadeInTime;
    public float creditDisplayTime;
    public float creditFadeOutTime;
    public TMP_Text credit;
    //public string text;
}
