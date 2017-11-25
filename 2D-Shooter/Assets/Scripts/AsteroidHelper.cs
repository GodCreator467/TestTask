using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHelper : MonoBehaviour
{

    private float speed = 0.1f;//скорость движения
    private Vector2 _target;// направление движения
    [SerializeField]
    private int healthPoint = 50;// текущее кол-во хп
    public int HealthPoint
    {
        get { return healthPoint; }
        set
        {
            healthPoint = value;//получение урона
            if (healthPoint <= 0)
                Destroy(gameObject);
        }
    }

    private void Start()
    {
        StaticData.CountAsteroid++; //наращиваем число астероидов на сцене
        _target = new Vector2(-StaticData._asteroidTransform.x, -StaticData._asteroidTransform.y); //начальное направление движения
        GetComponent<Rigidbody2D>().AddForce(_target * speed * Time.deltaTime); //задаем начальное ускорение
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<BorderHelper>()) //если астероид вышел за зону видимости камеры
        {
            Destroy(gameObject);//удаляем этот астероид
            StaticData.CountAsteroid--;
        }
        if (col.gameObject.GetComponent<AsteroidHelper>()) //если касаемся др. астероида
            StartCoroutine(СlashAsteroids(col));
    }
    IEnumerator СlashAsteroids(Collider2D col)
    {
        GetComponent<PolygonCollider2D>().isTrigger = false;//отключаем триггер для плавного столкновения
        yield return new WaitForSeconds(0.25f); //задержка отталкивания астероидов
        if (col && gameObject)   //если объекты еще не вылетели за зону видимости 
        {
            _target = gameObject.transform.position + (gameObject.transform.position - col.transform.position);
            GetComponent<Rigidbody2D>().AddForce(_target * speed * 2 * Time.deltaTime); //задаем направление ОТ др. астероида (удвоенное ввиду гашений энергий)
            GetComponent<PolygonCollider2D>().isTrigger = true;//включаем триггер снова для последующих столкновени
        }
    }
}
