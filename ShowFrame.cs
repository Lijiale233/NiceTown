using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ShowDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        onActivate.AddListener(ShowFrame);
        onDeactivate.AddListener(HideFrame);
    }

    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    public GameObject DialogueFrame;
    public TextMeshProUGUI DialogueMessage;
    //public bool Activated ;
    //呈现出对话框
    public void ShowFrame()
    {
        
            Debug.Log("function call success");
            Debug.Log("click success!");
            DialogueFrame.SetActive(true);
            //DialogueMessage.text = "nice to meet u";
            //Debug.Log("nice to meet u");
        
    }

    public void Activate()
    {
        Debug.Log("IsTrigged function call succeed");
        onActivate.Invoke();
    }

    public void DeActivate()
    {
        Debug.Log("DeActivate function activated");
        onDeactivate.Invoke();
    }
    public void ReceiveText(string text)
    {
        Debug.Log("ReceiveData function activated");
        DialogueMessage.text = text;//接收要呈现出的文本信息
    }
    public void HideFrame()
    {
        Debug.Log("HideFrame implemented");
   
        DialogueFrame?.SetActive(false);
        Debug.Log("Hide Frame activated");
        

            
    }
}
