using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHelper : MonoBehaviour
{
    private GameObject asteroid;//префаб астероида
    private float popravka;//поправка на размер астероида (в момент респа его нет на экране)
    private Vector2 bottomLeft; //координаты углов экрана
    private Vector2 topLeft;
    private Vector2 topRight;

    // Use this for initialization
    void Awake()
    {
        StaticData.Asteroids = new GameObject[10];//инициализация массива астероидов
        SizeOfScreen();//размер экрана
        StartCoroutine(Spawn());
    }

    private void SizeOfScreen()
    {
        Camera camera = GetComponent<Camera>(); //камера для опр. границ экрана
        //Три крайние точки камеры (по углам экрана)           
        bottomLeft = camera.ScreenToWorldPoint(new Vector2(0, 0));
        topLeft = camera.ScreenToWorldPoint(new Vector2(0, camera.pixelHeight));
        topRight = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, camera.pixelHeight));
    }
    private void Update()
    {
        CreateNLO();//создание NLO по клику
    }

    private void CreateNLO()
    {
        if (Input.GetMouseButtonDown(0)) //если нажат левый клик мышки
        {
            Ray hit = Camera.main.ScreenPointToRay(Input.mousePosition);//пускаем луч по месту клика для проверки на занятость и респа NLO
            Collider[] colliders = Physics.OverlapSphere(hit.origin, popravka); //радиус проверки на наличие объектов -по самому большому (астероид)
            if (colliders.Length == 0)//если луч не попал ни в какой объект 
            {
                Vector3 pointPosition = hit.origin; //координаты клика
                pointPosition.z = 0;//в откомпиллированном варианте координата z не ноль (почему?- не ясно)
                StaticData.NLOs.Add(Instantiate(Resources.Load<GameObject>("NLO"), pointPosition, Quaternion.identity));
            }
        }
    }

    IEnumerator Spawn()
    {
        StaticData._asteroidTransform = Vector2.zero;
        asteroid = Resources.Load<GameObject>("Asteroid");//объект астероида    
        popravka = asteroid.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        yield return new WaitForSeconds(1); //задержка старта игры 1с
        while (true)
        {
            StaticData._asteroidTransform.x = UnityEngine.Random.Range(topLeft.x, topRight.x); //респ по горизонтали в любом месте
            StaticData._asteroidTransform.y = (UnityEngine.Random.Range(0, 2) == 0) ? (bottomLeft.y - popravka) : (topLeft.y + popravka); //высота респа либо 0 либо самый верх
            if (StaticData.CountAsteroid < 10)//если астероидов на сцене менее 10
            {
                int count = 0; //временный счетчик массива
                foreach (GameObject item in StaticData.Asteroids) //в пустой элемент массива астероидов даем ссылку на новый астероид
                {
                    if (item == null)
                    {
                        StaticData.Asteroids[count] = Instantiate(asteroid, StaticData._asteroidTransform, Quaternion.identity);
                        break;
                    }
                    count++;
                }
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(5.0F, 10.0F));//задержка последующего спауна астероида 5-10сек.
        }
    }
}



