using UnityEngine;
using UnityEngine.Rendering;


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