using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignPopup : MonoBehaviour
{
    public GameObject popupGO;
    [TextArea(1,3)] public string text;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            popupGO.SetActive(true);
            TextMeshProUGUI goText = popupGO.GetComponentInChildren<TextMeshProUGUI>();
            goText.text = text;
            popupGO.GetComponent<Animator>().SetTrigger("Popup");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popupGO.GetComponent<Animator>().SetTrigger("Popout");
        }
    }
}
