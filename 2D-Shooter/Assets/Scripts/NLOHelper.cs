using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLOHelper : MonoBehaviour
{
    [SerializeField]
    private int healthPoint = 200;// текущее кол-во хп
    private Animator animator;//аниматор объекта
    private float speed = 2.0f;//скорость NLO (20% от мин. скорости астероида)
    private GameObject _targetMove = null;//цель для следования
    private GameObject _targetAttack = null;//цель для атаки
    private float distanceAttack = float.MaxValue; //дистанция до ближайшей потенциальной цели для аттаки
    private float distanceMove = float.MaxValue; //дистанция до ближайшей потенциальной цели для аттаки
    private float _time;//время след. запуска снаряда (раз в 0.5сек)
    public int _number;//номер НЛО в массиве
    public GameObject _parent;
    public List<GameObject> _shells = new List<GameObject>();//список снарядов NLO
    public int HealthPoint
    {
        get { return healthPoint; }
        set
        {
            healthPoint = value;//получение урона
            if (healthPoint <= 0)
                Death();
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        string trigger = (UnityEngine.Random.Range(0, 2) == 0) ? "leftrotate" : "rightrotate";
        animator.SetTrigger(trigger); //НЛО крутится вправо или влево
    }

    void Update()
    {
        distanceAttack = float.MaxValue; //каждый кадр сбрасываем прошлые данные
        distanceMove = float.MaxValue; //

        MoveToTarget();//движение к ближайшему NLO   
        AttackToTarget(); //атака по ближайшему объекту   

        if (_targetAttack != null && _time < Time.time && GetComponentInParent<NLOPointHelper>().enabled)//если есть хоть одна цель
            Attack();//проводим атаку
    }

    private void AttackToTarget()
    {

        for (int i = 0; i < StaticData.Asteroids.Length; i++) //перебор на поиск цели для атаки
        {
            if (StaticData.Asteroids[i] != null)
            {
                float temp;//расстояние до астероидов
                temp = Vector2.Distance(transform.position, StaticData.Asteroids[i].transform.position);

                if (temp < distanceAttack
                    && !gameObject.Equals(StaticData.Asteroids[i].GetComponentInChildren<NLOHelper>())
                    && temp < (GetComponent<SpriteRenderer>().bounds.size.x * 5)
                    && temp < distanceMove) //ищем ближайшую цель
                {
                    distanceAttack = temp;
                    _targetAttack = StaticData.Asteroids[i];
                }
            }
        }
    }
    private void Attack()
    {
        _time = Time.time + 0.5f;//след. снаряд через 0.5сек
        GameObject _shell = Resources.Load<GameObject>("Shell");//префаб снаряда
        _shell.GetComponent<ShellHelper>()._parent = gameObject;//даем знать снаряду кто его запустил 
        //назначаем целью снаряда требуемую координату (при чем целимся в цель, но координату назначаем за нею интерполяцией вектора, имитирую реальную пушечную стрельбу)      
        _shell.GetComponent<ShellHelper>()._target = 100 * (_targetAttack.transform.position) - 99 * transform.position;
        _shells.Add(Instantiate(_shell, transform.position, Quaternion.identity)); //инстанцируем снаряд с занесением в список

    }
    private void MoveToTarget()
    {

        foreach (GameObject obj in StaticData.NLOs)
        {
            if (!_parent.Equals(obj))
            {
                float tempMove;//расстояние между NLO
                tempMove = Vector2.Distance(GetComponentInParent<NLOPointHelper>().transform.position, obj.transform.position);

                if (tempMove < distanceMove) //ищем ближайшее NLO
                {
                    _targetMove = obj;
                    distanceMove = tempMove;
                }
            }
        }

        if (_targetMove != null)//если есть хоть одно другое NLO
        {
            GetComponentInParent<NLOPointHelper>().transform.position = Vector3.MoveTowards(GetComponentInParent<NLOPointHelper>().transform.position, _targetMove.transform.position, speed * Time.deltaTime);//движение осуществляем по прямой через MoveTowards
            _targetAttack = _targetMove;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<AsteroidHelper>() ||//если касаемся астероида или
            col.gameObject.GetComponent<NLOHelper>())  //если касаемся др. NLO
            Death();
    }

    private void Death()
    {
        Destroy(GetComponentInParent<NLOPointHelper>().gameObject, 1);//окончательный дестрой род. объекта через 1сек (время анимации)
        GetComponent<CircleCollider2D>().enabled = false; //отключение компонентов объекта, взаимодействующих с миром
        GetComponentInParent<NLOPointHelper>().enabled = false;
        StaticData.NLOs.Remove(GetComponentInParent<NLOPointHelper>().gameObject);
        animator.SetTrigger("death"); //анимация смерти
    }
}
