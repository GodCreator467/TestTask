using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellHelper : MonoBehaviour
{
    public GameObject _parent; //оператор стрельбы
    public Vector3 _target;
    private float speedShell = 13.5f;//скорость снаряда (2-3 диаметра NLO в сек)
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, speedShell * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.GetComponent<AsteroidHelper>()) //если касаемся астероида                
        {
            col.gameObject.GetComponent<AsteroidHelper>().HealthPoint -= 10;
            Death();
        }
        if (col.gameObject.GetComponent<BorderHelper>()) //если улетели за границу сцены             
            Death();
        if (col.gameObject.GetComponent<NLOHelper>() && !col.gameObject.Equals(_parent))  //если касаемся др. NLO (и это не NLO оператор) 
        {
            col.gameObject.GetComponent<NLOHelper>().HealthPoint -= 10;
            Death();
        }


    }
    void Death()
    {
        if (_parent)
            _parent.GetComponentInChildren<NLOHelper>()._shells.Remove(gameObject);//удаляем снаряд из списка снарядов НЛО, запустившего его
        Destroy(gameObject);
    }
}