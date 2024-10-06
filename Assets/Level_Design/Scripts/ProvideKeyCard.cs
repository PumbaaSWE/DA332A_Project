using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvideKeyCard : MonoBehaviour
{
    [SerializeField] GameObject keyCard;
    bool givenCard = false;

    public void GiveKeyCard()
    {
        if (!givenCard)
        {
            keyCard.SetActive(true);
            givenCard = true;
        }

    }
}
