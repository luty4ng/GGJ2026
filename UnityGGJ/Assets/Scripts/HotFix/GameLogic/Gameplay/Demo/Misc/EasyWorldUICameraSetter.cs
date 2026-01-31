using UnityEngine;

public class EasyWorldUICameraSetter : MonoBehaviour
{
    void Awake()
    {
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}