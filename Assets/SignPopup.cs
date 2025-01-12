using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignPopup : MonoBehaviour
{
    public GameObject popupGO;
    public string text;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            popupGO.SetActive(true);
            TextMeshProUGUI goText = popupGO.GetComponentInChildren<TextMeshProUGUI>();
            goText.text = text;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popupGO.SetActive(false);
        }
    }
}
