using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public static float Swidth; //ширина экрана
    public static float Sheight;//высота экрана
    public static Vector2 _asteroidTransform;//координаты астероида
    public static int CountAsteroid;//число астероидов на сцене
    public static GameObject[] Asteroids;//массив астероидов
    public static List<GameObject> NLOs = new List<GameObject>();//список NLO
}

