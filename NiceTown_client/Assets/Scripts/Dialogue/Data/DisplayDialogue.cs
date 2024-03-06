using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GetDialogues;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DisplayDialogue : MonoBehaviour
{
    [Header("���ŶԻ����")]
    //delayBetweenCharacters ÿ����������ٶ�
    public float delayBetweenCharacters = 0.1f;
    //delayAfterSentence �����һ�������Ļ�����ʾʱ��
    public float delayAfterSentence = 1.0f;
    //textData�����ݴ���һ��Ҫ�������������
    public DialogueData textData=new DialogueData();
    //������NPC����<=���ֵ��ʼ����
    public float triggerDistance = 5.0f;
    //��¼�ܹ��Ĳ���ʱ��
    public float totalDialogueTime = 0f;

    [HeaderAttribute("ui���")]
    public Image FaceImage;//ͷ��
    public TextMeshProUGUI DialogueText;//չʾ����������
    public GameObject frame;//ͷ��+չʾ�ı��������


    private GameObject NPC_1, NPC_2;
    
    public void Initialize(int session)
    {
        textData = Dialogue.MatchDialogues[session];
        //����Agent���ҵ���Ӧ��gameObject
        NPC_1 = GameObject.Find(textData.Subject);
        NPC_2 = GameObject.Find(textData.Object);
    }
    //������npc�����ж��Ƿ����չ������������
    public bool AllowToDisplay()
    {
        if (NPC_1.activeInHierarchy && NPC_2.activeInHierarchy)
        {
            float distance = Vector3.Distance(NPC_1.transform.position, NPC_1.transform.position);
            if (distance < triggerDistance)
            {
                Debug.Log("����չ���Ի�");
                return true;
            }
        }
        return false;
    }
    //ʱʱ��飬����������Ҫ�󡢳���Ҫ����ʼ���в���
    private void Update()
    {
        if (AllowToDisplay())
        {
            Debug.Log("׼�����ŶԻ�����");
            DisplayTextContent();
        }
    }
    public void Awake()
    {

    }
    
    //TypeSentence չʾ�Ի�����
    IEnumerator TypeSentence(string sentence)
    {
        Debug.Log("��ʼ��ӡһ�仰");
        float startTime = Time.time;
        DialogueText.text = "";
        foreach (char character in sentence)
        {
            DialogueText.text += character;
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
        //���¶Ի�����ʱ��
        totalDialogueTime += Time.time - startTime + delayAfterSentence;
    }
    //���Ի�����д������
    IEnumerator TypeDialogue()
    {
        Debug.Log("writing dialogues");
        foreach(var dialogues in textData.chat)
        {
            //����Ի�ʱ�䳬��ֱ�ӽ����ⲿ��
            if (totalDialogueTime > textData.duration)
                break;
            string sentence = dialogues.dialog;
            yield return StartCoroutine(TypeSentence(sentence));
            yield return new WaitForSeconds(delayAfterSentence);

        }
        Debug.Log("finish writing");
    }
    //��ʼ����Ի�����ʱ��������Ϊ�ɼ�
    //�����������Ϊ���ɼ�
    public void DisplayTextContent()
    {
        frame.SetActive(true);
        StartCoroutine(TypeDialogue());
        frame.SetActive(false);
    }
}
