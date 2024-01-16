using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGaugeFunctions : MonoBehaviour
{
    public Animator bulbAnimator;
    public GameObject gaugeArrow;

    public float gaugeSpeed;
    public GameObject[] gaugeLights;

    public void CheckHealth(float currentHealth, float maxHealth)
    {
        for (int i = 0; i < gaugeLights.Length; i++)
        {
            if (i * (1/11) <= currentHealth / maxHealth)
            {
                gaugeLights[i].SetActive(true);
            }
            else
            {
                gaugeLights[i].SetActive(false);
            }
        }

        StartCoroutine(ChangeGauge(currentHealth, maxHealth));

        if (currentHealth / maxHealth <= 0.25f)
        {
            TriggerLight(true);
        }
        else
        {
            TriggerLight(false);
        }
    }

    private void TriggerLight(bool state)
    {
        bulbAnimator.SetBool("Light On", state);
    }

    private IEnumerator ChangeGauge(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        float newRotation = 0;

        while (newRotation < 1 - healthPercent)
        {
            newRotation -= 1 - healthPercent * gaugeSpeed;
            gaugeArrow.transform.eulerAngles = new Vector3(0, 0, newRotation);
        }
        yield return null;
    }
}
