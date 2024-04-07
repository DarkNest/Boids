using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public BoidsGroup group;
    [HideInInspector]
    public BoidSetting setting;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 acceleration;
    [HideInInspector]
    public Vector3 aliAcc;
    [HideInInspector]
    public Vector3 sepAcc;
    [HideInInspector]
    public Vector3 cohAcc;
    [HideInInspector]
    public int perceptNum;


    public void Init(BoidsGroup boidsGroup, Transform targetTrans)
    {
        target = targetTrans;
        group = boidsGroup;
        setting = boidsGroup.setting;
        velocity = transform.forward * Random.Range(setting.minSpeed, setting.maxSpeed);
    }

    public void BoidUpdate(float deltaTime)
    {
        acceleration = Vector3.zero;

        //计算加速度
        acceleration += CalcuTargetAcceleration();
        acceleration += CalcuBoidAcceleration();
        acceleration += CalcuCollisionAcceleration();

        velocity = velocity + acceleration * deltaTime;
        float speed = Mathf.Clamp(velocity.magnitude, setting.minSpeed, setting.maxSpeed);
        velocity = velocity.normalized * speed;

        Vector3 cur = transform.position;
        Vector3 tar = cur + velocity * deltaTime;
        transform.position = tar;
        transform.forward = velocity.normalized;
    }

    private Vector3 CalcuTargetAcceleration()
    {
        Vector3 acc = Vector3.zero;
        if (target != null)
        {
            //朝向目标点加速度
            Vector3 tarAcc = target.position - transform.position;
            acc = SteerTowards(tarAcc) * setting.targetWeight;
        }
        return acc;
    }

    private Vector3 CalcuBoidAcceleration()
    {
        List<Boid> boids = group.boids;
        Vector3 acc = Vector3.zero;

        //不使用 ComputeShader 计算
        if (!setting.useComputerShader)
        {
            aliAcc = Vector3.zero;
            sepAcc = Vector3.zero;
            cohAcc = Vector3.zero;
            perceptNum = 0;

            foreach (Boid boid in boids)
            {
                if (boid == this)
                    continue;
                Vector3 delta = boid.transform.position - transform.position;
                float dis = delta.magnitude;

                if (dis < setting.perceptionRadius)
                {
                    perceptNum++;
                    cohAcc += delta;
                    //Alignment加速度
                    aliAcc += boid.transform.forward;
                    //Separation加速度
                    if (dis < setting.avoidanceRadius)
                        sepAcc -= delta.normalized / dis;
                }
            }
        }
        
        if (perceptNum != 0)
        {
            //Cohesion加速度
            cohAcc = cohAcc / perceptNum;
            acc = SteerTowards(aliAcc) * setting.alignmentWeight + 
                  SteerTowards(sepAcc) * setting.separatonWeight +
                  SteerTowards(cohAcc) * setting.cohesionWeight;
        }
        
        return acc;
    }

    private Vector3 CalcuCollisionAcceleration()
    {
        Vector3 acc = Vector3.zero;
        RaycastHit hitInfo;

        //射线检测正前方，出现阻挡物时，进行后续检测
        bool ishit = Physics.SphereCast(transform.position,
            setting.collisionRadius,
            transform.forward,
            out hitInfo,
            setting.collisionDistance,
            setting.collisionMask);
        if (!ishit)
            return acc;

        //获取需要检测的射线方向（Local坐标）
        Vector3[] detectDir = group.GetDetectDirs();
        for(int i = 0; i < detectDir.Length; i++)
        {
            Vector3 dir = detectDir[i];
            //将射线转化到世界坐标
            dir = transform.TransformDirection(dir);            
            ishit = Physics.SphereCast(transform.position,
                setting.collisionRadius,
                dir,
                out hitInfo,
                setting.collisionDistance,
                setting.collisionMask);
            if (ishit)
                continue;
            //朝向没有碰撞的方向加速
            acc = dir;
            break;
        }
        acc = SteerTowards(acc) * setting.collisionWeight;
        
        return acc;
    }

    /// <summary>
    /// 限制最大加速度
    /// </summary>
    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * setting.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, setting.maxAccelation);
    }

    private void OnDrawGizmos()
    {
        //Debug
        //var dirs = group.GetDetectDirs();
        //foreach(var dir in dirs)
        //{
        //    Vector3 d = transform.TransformDirection(dir);
        //    Vector3 p = transform.position;
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(p, p + d * setting.collisionDistance);
        //}
    }
}
