using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsGroup : MonoBehaviour
{
    private const int threadNum = 1024;

    public Transform target;
    public GameObject prefab;
    public ComputeShader comShader;
    public BoidSetting setting;

    [HideInInspector]
    public List<Boid> boids = new List<Boid>();
    private Vector3[] detectDir;
    private BoidData[] datas;

    private void Awake()
    {
        for (int i = 0; i < setting.genNum; i++)
        {
            Transform trans = Instantiate(prefab).transform;
            trans.parent = transform;
            trans.localPosition = Random.insideUnitSphere * setting.genRadius;
            trans.forward = Random.insideUnitSphere;
            Boid boid = trans.GetComponent<Boid>();
            boid.Init(this, target);
            boids.Add(boid);
        }
        datas = new BoidData[setting.genNum];
    }

    private void Update()
    {
        if (setting.useComputerShader)
        {
            if (comShader == null)
            {
                Debug.LogError("Cannot find computeShader");
                return;
            }

            //封装数据
            for (int i = 0; i < boids.Count; i++)
            {
                Boid boid = boids[i];
                datas[i].position = boid.transform.position;
                datas[i].foward = boid.transform.forward;
                datas[i].aliAcc = Vector3.zero;
                datas[i].sepAcc = Vector3.zero;
                datas[i].cohAcc = Vector3.zero;
                datas[i].perceptNum = 0;
            }
            ComputeBuffer buffer = new ComputeBuffer(datas.Length, BoidData.GetSize());
            buffer.SetData(datas);

            comShader.SetBuffer(0, "boids", buffer);
            comShader.SetInt("boidNum", boids.Count);
            comShader.SetFloat("perceptionRadius", setting.perceptionRadius);
            comShader.SetFloat("avoidanceRadius", setting.avoidanceRadius);
            int threadGroupNum = Mathf.CeilToInt((float)boids.Count / threadNum);            
            comShader.Dispatch(0, threadGroupNum, 1, 1);

            buffer.GetData(datas);
            buffer.Release();

            //设置数据
            for (int i = 0; i < boids.Count; i++)
            {
                Boid boid = boids[i];
                boid.aliAcc = datas[i].aliAcc;
                boid.sepAcc = datas[i].sepAcc;
                boid.cohAcc = datas[i].cohAcc;
                boid.perceptNum = datas[i].perceptNum;
            }
        }

        foreach(Boid boid in boids)
        {
            boid.BoidUpdate(Time.deltaTime);
        }        
    }

    public Vector3[] GetDetectDirs()
    {
        if (detectDir == null || true)
        {
            int numDirs = setting.detectNum;
            //均匀球面离散点生成
            //参考文章:https://people.engr.tamu.edu/schaefer/research/normalCompression.pdf
            detectDir = new Vector3[numDirs];
            float range = Mathf.Cos(setting.collisionAngle * Mathf.PI / 360);
            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;
            for (int i = 0; i < numDirs; i++)
            {
                Vector3 p = Vector3.zero;
                float t = (float)i / numDirs;

                //扇形区域
                float ac = 1 - (1 - range) * t;
                //球形区域
                //float ac = 1 - 2 * t;

                //计算射线方向
                float alpha = Mathf.Acos(ac);
                float beta = angleIncrement * i;
                p.x = Mathf.Sin(alpha) * Mathf.Cos(beta);
                p.y = Mathf.Sin(alpha) * Mathf.Sin(beta);
                p.z = Mathf.Cos(alpha);
                detectDir[i] = p;
            }
        }

        return detectDir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setting.genRadius);

        ///////////////////////////////////////Debug
        //if (boids.Count == 0)
        //    return;
        //Boid boid = boids[0];
        //RaycastHit hitInfo;

        //bool isHit = Physics.SphereCast(boid.transform.position,
        //    setting.collisionRadius,
        //    boid.transform.forward,
        //    out hitInfo,
        //    setting.collisionDistance,
        //    setting.collisionMask);
        //Gizmos.color = isHit ? Color.red : Color.green;
        //Gizmos.DrawWireSphere(boid.transform.position + boid.transform.forward * hitInfo.distance, setting.collisionRadius);

        //Vector3[] detectDir = GetDetectDirs();
        //for (int i = 0; i < detectDir.Length; i++)
        //{
        //    Vector3 dir = detectDir[i];
        //    dir = boid.transform.TransformDirection(dir).normalized;

        //    bool ishit;
        //    ishit = Physics.SphereCast(boid.transform.position,
        //        setting.collisionRadius,
        //        dir,
        //        out hitInfo,
        //        setting.collisionDistance,
        //        setting.collisionMask);
        //    Gizmos.color = ishit ? Color.red : Color.green;
        //    Vector3 from = boid.transform.position;
        //    Vector3 to = from + dir * setting.collisionDistance;
        //    Gizmos.DrawLine(from, to);
        //}
    }
}
