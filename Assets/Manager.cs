using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Manager : MonoBehaviour
{
    public List<Particles> Planets { get; private set; } = new List<Particles>();
    //public List<GameObject> tela;
    //public GameObject [] array ;
    public float gravityConst = 6.67f;//6.67e-11f;
    public Vector3 centr;
    public float scale = 1;
    public float dt = 0.02f;
    //public double rKUO = 100000000000;
    public int k = 1;
    public GameObject Prefab;
    public GameObject PrefabKUO;

        //GameObject [] array = GameObject.FindGameObjectsWithTag("telo");
        //tela = array.ToList();
    //private Stopwatch _calculatingWatch = new Stopwatch();//Замер производительности

    private const int treads_count = 20;//количество потоков
    private Thread[] _updatingThreads = new Thread[treads_count];
    private SemaphoreSlim _startUpdating = new SemaphoreSlim(0);
    private Barrier _updatingBarrier = new Barrier(treads_count);

    void Start()
    {
        for (int i = 0; i < 800; i++)
        {
            NewBody(PrefabKUO, 50);
        }
        for (int i = 0; i < _updatingThreads.Length; i++)
        {
            _updatingThreads[i] = new Thread(parameter => UpdateThreadStarted((int)parameter, treads_count));//Создаём новый поток с индексом i
            _updatingThreads[i].Start(i + 1);//передаем в функцию UpdateThreadStarted переменную parameter=(i+1) текущий индекс потока и количество потоков treads_count
        }
    }
    private void UpdateThreadStarted(int index, int count)
    {
        while (true)
        {
            _startUpdating.Wait();
            UpdateThread(index, count);
            _updatingBarrier.SignalAndWait();
        }
    }
    private void UpdateThread(int packIndex, int totalPacks)
    {   //разобъём список планет на равные доли пропорционально количеству потоков 
        float from = (float)(packIndex - 1) / totalPacks;// коэфф начала 
        float to = (float)(packIndex) / totalPacks;      // коэф конца

        for (int i = (int)(Planets.Count * from); i < (int)(Planets.Count * to); i++)

        { 
            Particles part = Planets[i];
            part.Gravity();

        }

        _updatingBarrier.SignalAndWait();
    }
    private void ApplyForces()
    {
        foreach (Particles planet in Planets)
        {
            planet.ApplyForce();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _startUpdating.Release(treads_count);
        ApplyForces();
        Baricentr();
        if (Input.GetKey(KeyCode.Space))
        {
            NewBody(Prefab, 20);
        }
    }
    private Vector3 GetScaledPos(Vector3 Pose)
    {
        var scaledPos = Pose / scale;
        return new Vector3(Convert.ToSingle(scaledPos.x),
                           Convert.ToSingle(scaledPos.y),
                           Convert.ToSingle(scaledPos.z));
    }
    private void NewBody(GameObject pref,int rang)
    {

        float x = UnityEngine.Random.Range(-rang,rang);
        float y = UnityEngine.Random.Range(-rang,rang);            
        float z = UnityEngine.Random.Range(-rang,rang);
        GameObject newObject = Instantiate(pref, new Vector3(x, y, z), Quaternion.identity);
        newObject.name = pref.name+(Planets.Count+1).ToString();
    }
    private void Baricentr()
    {
        Vector3 Pose = new Vector3(0, 0, 0);
        float mas = 0;
        for (int i = 0; i < Planets.Count; i++)
        {
            Particles P = Planets[i];
            //Debug.Log("P.Pos" + P.Pos);
            if (P.Pos.x != float.NaN)
            {
                Pose = Pose + P.Pos * P.mass;// (m1x1 + m2x2 + m3x3) / (m1 + m2 + m3)
                mas = mas + P.mass;
            }
        }
        if (mas != 0)
        {
            centr = GetScaledPos(Pose / mas);
            transform.position = centr;
        }
    }
}
