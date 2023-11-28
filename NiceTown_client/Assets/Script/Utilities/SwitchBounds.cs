using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //TODO: �л���������
    private void OnEnable()
    {
        EventHandler.TransitionEvent += event2;
    }

    private void event2(string arg1, Vector3 arg2)
    {
        print("ע��ĵڶ����¼�");
    }


    private void Start()
    {
        SwitchConfinerShape();
    }
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent <CinemachineConfiner> ();
        confiner.m_BoundingShape2D = confinerShape;

        confiner.InvalidatePathCache();
    }
}
