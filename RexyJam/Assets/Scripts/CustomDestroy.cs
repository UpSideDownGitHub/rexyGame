using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.Table;

public class CustomDestroy : MonoBehaviour
{
    public float destroyTime;

    public void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}