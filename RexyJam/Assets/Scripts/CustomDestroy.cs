using UnityEngine;
using UnityEngine.Rendering;


public class CustomDestroy : MonoBehaviour
{
    public float destroyTime;

    public void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}