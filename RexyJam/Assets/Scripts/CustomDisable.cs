using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.Table;

public class CustomDisable : MonoBehaviour
{
    public float disableTime;

    public void OnEnable ()
    {
        Invoke("Disable", disableTime);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}