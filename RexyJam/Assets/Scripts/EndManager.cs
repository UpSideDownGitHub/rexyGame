using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public Animator animator1;
    public GameObject other;
    public float givenTime;

    public void OnEnable()
    {
        animator1.SetTrigger("PLAY");
        other.SetActive(true);
        Invoke("EndGame", givenTime);
    }

    public void EndGame()
    {
        SceneManager.LoadSceneAsync("Credits");
    }
}
