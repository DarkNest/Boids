using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class BoidSetting : ScriptableObject
{
    [Header("=====Optimize=====")]
    public bool useComputerShader = true;   //使用ComputerShader优化

    [Header("=====Generation=====")]
    public float genRadius = 10f;           //生成半径    
    public int genNum = 100;                //生成数量

    [Header("=====Athletic Status=====")]
    public float maxAccelation = 10f;       //最大加速度
    public float minSpeed = 2f;             //最小速度
    public float maxSpeed = 5f;             //最大速度

    [Header("=====Perception=====")]
    public float perceptionRadius = 3f;     //感知半径
    public float avoidanceRadius = 2f;      //回避半径

    public float targetWeight = 0.5f;       //目标权重
    public float alignmentWeight = 0.5f;    //平行权重
    public float separatonWeight = 0.5f;    //分离权重    
    public float cohesionWeight = 0.5f;     //内聚权重

    [Header("=====Collision=====")]
    public LayerMask collisionMask;         //碰撞检测层
    public int detectNum = 50;              //碰撞检测数量
    public float collisionAngle = 180;      //碰撞感知角
    public float collisionRadius = 0.3f;    //碰撞检测半径
    public float collisionDistance = 5f;    //碰撞检测距离
    public float collisionWeight = 20f;     //碰撞检测权重
}
