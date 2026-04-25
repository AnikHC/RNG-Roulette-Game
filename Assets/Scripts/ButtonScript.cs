using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    //To make the button change color when hovered over, and change back when not hovered over. 
    // Only changes color if the button is interactable.   
    public void OnPointerEnter(PointerEventData eventData){
        if(gameObject.GetComponent<Button>().interactable){
            Color color;
            if(ColorUtility.TryParseHtmlString("#FFEAAE", out color)){
                buttonImage.color = color;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData){
       if(gameObject.GetComponent<Button>().interactable){
            Color color;
            if(ColorUtility.TryParseHtmlString("#FFDC7B", out color)){
                buttonImage.color = color;
            }
        }
    }
}
