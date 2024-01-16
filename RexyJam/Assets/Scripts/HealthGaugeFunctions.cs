using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthGaugeFunctions : MonoBehaviour
{
    public Animator bulbAnimator;
    public Animator bobbleHeadAnimator;
    public Animator newWaveBulbs;
    public Animator multiplierBulbs;

    public GameObject gaugeArrow;

    public float gaugeSpeed;
    public GameObject[] gaugeLights;

    public TMP_Text[] healthNixieTexts;

    public void CheckHealth(float currentHealth, float maxHealth)
    {
        bobbleHeadAnimator.SetTrigger("Shake");

        string health = currentHealth.ToString();
        var healthList = health.ToCharArray();

        for (int i = 2; i >= 0; i--)
        {
            if (healthList.Length > i)
                healthNixieTexts[i].text = healthList[i].ToString();
            else
                healthNixieTexts[i].text = "0";
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

        StopAllCoroutines();
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
        float targetRotation = 180 * (1 - healthPercent);
        float currentRotation = gaugeArrow.transform.eulerAngles.z;
        int failSafe = 0;


        
        gaugeArrow.GetComponent<Animator>().enabled = false;
        if (currentRotation > targetRotation)
        {
            while (currentRotation > targetRotation)
            {
                failSafe++;
                currentRotation -= gaugeSpeed * Time.deltaTime;
                gaugeArrow.transform.eulerAngles = new Vector3(0, 0, currentRotation);
                yield return null;
                if (failSafe >= 200)
                    break;
            }
        }
        else
        {
            while (currentRotation < targetRotation)
            {
                failSafe++;
                currentRotation += gaugeSpeed * Time.deltaTime;
                gaugeArrow.transform.eulerAngles = new Vector3(0, 0, currentRotation);
                yield return null;
                if (failSafe >= 200)
                    break;
            }
        }
        gaugeArrow.GetComponent<Animator>().enabled = true;
    }

    public void NewWaveBulbs()
    {
        newWaveBulbs.SetTrigger("New Wave");
    }

    public void SetMultiplierBulbs(bool active)
    {
        multiplierBulbs.SetBool("Multiplier Active", active);
    }
}
