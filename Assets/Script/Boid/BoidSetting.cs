using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class BoidSetting : ScriptableObject
{
    [Header("=====Optimize=====")]
    public bool useComputerShader = true;   //ʹ��ComputerShader�Ż�

    [Header("=====Generation=====")]
    public float genRadius = 10f;           //���ɰ뾶    
    public int genNum = 100;                //��������

    [Header("=====Athletic Status=====")]
    public float maxAccelation = 10f;       //�����ٶ�
    public float minSpeed = 2f;             //��С�ٶ�
    public float maxSpeed = 5f;             //����ٶ�

    [Header("=====Perception=====")]
    public float perceptionRadius = 3f;     //��֪�뾶
    public float avoidanceRadius = 2f;      //�رܰ뾶

    public float targetWeight = 0.5f;       //Ŀ��Ȩ��
    public float alignmentWeight = 0.5f;    //ƽ��Ȩ��
    public float separatonWeight = 0.5f;    //����Ȩ��    
    public float cohesionWeight = 0.5f;     //�ھ�Ȩ��

    [Header("=====Collision=====")]
    public LayerMask collisionMask;         //��ײ����
    public int detectNum = 50;              //��ײ�������
    public float collisionAngle = 180;      //��ײ��֪��
    public float collisionRadius = 0.3f;    //��ײ���뾶
    public float collisionDistance = 5f;    //��ײ������
    public float collisionWeight = 20f;     //��ײ���Ȩ��
}
