using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthGaugeFunctions : MonoBehaviour
{
    public Animator bulbAnimator;
    public GameObject gaugeArrow;

    public float gaugeSpeed;
    public GameObject[] gaugeLights;

    public TMP_Text[] healthNixieTexts;

    public void CheckHealth(float currentHealth, float maxHealth)
    {
        if (currentHealth == 100)
        {
            healthNixieTexts[0].text = "1";
            healthNixieTexts[1].text = "0";
            healthNixieTexts[2].text = "0";
        }
        else
        {
            string health = currentHealth.ToString();
            var healthList = health.ToCharArray();

            healthNixieTexts[0].text = "0";

            for (int i = 0; i < healthList.Length; i++)
            {
                healthNixieTexts[i + 1].text = healthList[i].ToString();
            }
        }

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
