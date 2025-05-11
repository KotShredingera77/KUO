using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;


public class Particles : MonoBehaviour
{
    // Start is called before the first frame update
    public float mass;
    public Vector3 velocity;
    public Vector3 Pos;
    public Vector3 position;
 
    private Transform _transfrom;



    [SerializeField] private Manager manager;     //Класс массивом объектов и гравитационной постоянной
    private float dt;
    [SerializeField] private float v;
    [SerializeField] private float rKUO = 200;

    [SerializeField] int dir;
    [SerializeField] int tik = 0;    
    [SerializeField] private bool start = true;


    void Awake()
    {
        _transfrom = GetComponent<Transform>();

        position = _transfrom.position;

        manager = GameObject.Find("Centr").GetComponent<Manager>(); 
        dt = manager.dt;
        Pos = new Vector3(this.transform.position.x*manager.scale, this.transform.position.y * manager.scale, this.transform.position.z * manager.scale);
        NewVelosity();
         //Time.timeScale = 10;
    }
    private void OnEnable()
    {
       _transfrom = GetComponent<Transform>();
        position = _transfrom.position;

        manager = GameObject.Find("Centr").GetComponent<Manager>(); 
        dt = manager.dt;
        Pos = new Vector3(this.transform.position.x*manager.scale, this.transform.position.y * manager.scale, this.transform.position.z * manager.scale);
        NewVelosity();

        FindObjectOfType<Manager>().Planets.Add(this);
    }

    private void OnDisable()
    {
        if (gameObject.scene.isLoaded)
        {
            FindObjectOfType<Manager>().Planets.Remove(this);
        }
    }
    public void Gravity()
    {
        if (start) { start = NewVelosity(); }//начальная скорость

        Vector3 acselerator = new Vector3(0, 0, 0);

        for (int i = 0; i < manager.Planets.Count; i++)  //Обход массива объектов
        {
            Particles part = manager.Planets[i];

            if (!NeedsToBeProcessed(part)) { continue; } //Проверка исключительных ситуаций

            Vector3 direction = Pos - part.Pos;//вектор между телами
            float radius = Mag(direction);//растояние скаляр
            acselerator += manager.k * (manager.gravityConst * part.mass * direction / (radius * radius * radius));  //ускорение+

           // if (radius < 4)
           // { isKinematic = false; }

        }




            if (velocity != null)
            {
                velocity = velocity + acselerator * dt; //Изменение скорости
                Pos = Pos + velocity * dt;
                float rDistans = Mag(Pos);
                if (rDistans > rKUO) //если выход за пределы сферы то ограничим сферой
                {
                    float coefficient = rKUO / rDistans;
                    Pos = Pos * coefficient;
                }
                tik = tik + 1;
                if (tik == 1)
                {
                    tik = 0;
                    position = GetScaledPos(); //ПРименение позиции текущему объекту с учетом масштаба

                }
                v = Mag(velocity);
            }

    }   
    public void ApplyForce()
    {

            _transfrom.position = position;


    }
    private float Mag(Vector3 direction) 
    { return (float)Math.Sqrt(direction.x * direction.x + direction.y * direction.y + direction.z * direction.z); }

    private bool NeedsToBeProcessed( Particles part) //Проверка исключительных ситуаций
    {
        bool rez = true;

        if (part == null) //Если данные объекта пустые то пропустить
        { return false; }

        if (part == this) //Если позиции объектов совпали то пропустить
        { return false; }
        try
        {
            if (Pos == part.Pos) //Если позиции объектов совпали то пропустить
           { return false; }           



            if (part.Pos == null) //Если данные объекта пустые то пропустить
            { return false; }

            if (part.mass == 0) //Если данные объекта пустые то пропустить
            { return false; }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
        return rez; 
    }
    private bool NewVelosity()
    {
        velocity = Vector3.Cross(Pos - manager.centr, new Vector3(0, 1, 0)).normalized*v;
         //
        return false;
        //if (manager.tela.Length != 0)
        //{
        //    double sysMass = 0;
        //    Vec3 sysVec = new Vec3(0,0,0);
        //    for (int i = 0; i < manager.tela.Length; i++)  //Объод массива объектов
        //    {
        //        GameObject go = manager.tela[i];
        //        Particles part = go.GetComponent<Particles>();
        //        if (!NeedsToBeProcessed(go, part)) { continue; } //Проверка исключительных ситуаций
        //        sysMass += part.mass;
        //        sysVec += part.Pos * part.mass;
        //    }
        //    if (sysMass != 0)
        //    {
        //        sysVec = sysVec / sysMass;
        //        Vec3 direction = Pos - sysVec;//вектор между телом и центром противостоящих масс
        //        double radius = Mag(direction);//радиус скаляр
        //        v = (float)Math.Sqrt(manager.gravityConst * sysMass / radius); //абсолютная первая космическая скорость для этого радиуса
        //        double deltamass = sysMass / (sysMass + mass);
        //        v = v * deltamass;
        //        velocity = new Vec3(0, 0, v * dir);
        //        return false;
        //    }
        //    else { return true; }
        //}
        //else
        //{
        //    return true;
        //}  //Ctrl+K, C — закомментировать выделенные строки в коде. Ctrl+K, U — раскомментировать выделенные строки в коде

    }
    private Vector3 GetScaledPos()
    {
        var scaledPos = Pos / manager.scale;
        return new Vector3(Convert.ToSingle(scaledPos.x),
            Convert.ToSingle(scaledPos.y),
            Convert.ToSingle(scaledPos.z));
    }
}
