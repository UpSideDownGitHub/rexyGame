using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthGaugeFunctions : MonoBehaviour
{
    public Animator bulbAnimator;
    public Animator bobbleHeadAnimator;
    public Animator newWaveBulbs;
    public Animator multiplierBulbs;

    [Header("Wave UI")]
    public TMP_Text[] waveBulbs;

    [Header("Wave Slider UI")]
    public Image waveSliderImage;

    [Header("Health UI")]
    public float gaugeSpeed;
    public GameObject gaugeArrow;
    public GameObject[] gaugeLights;
    public TMP_Text[] healthNixieTexts;

    [Header("Score UI")]
    public TMP_Text[] scoreBulbs;

    [Header("Muliplier UI")]
    public TMP_Text multiplierBulbsText;

    public void SetMultiplierUI(float multiplier)
    {
        // TO BE IMPLEMENTED
    }

    public void SetScoreUI(int score)
    {
        string scoreString = score.ToString();
        var scoreCharList = scoreString.ToCharArray();
        if (scoreBulbs.Length < scoreCharList.Length)
        {
            scoreBulbs[0].text = "9";
            scoreBulbs[1].text = "9";
            scoreBulbs[2].text = "9";
            scoreBulbs[3].text = "9";
            scoreBulbs[4].text = "9";
            scoreBulbs[5].text = "9";
            scoreBulbs[6].text = "9";
            return;
        }
        scoreBulbs[0].text = scoreCharList.Length - 7 >= 0 ? scoreCharList[scoreCharList.Length - 7].ToString() : "0";
        scoreBulbs[1].text = scoreCharList.Length - 6 >= 0 ? scoreCharList[scoreCharList.Length - 6].ToString() : "0";
        scoreBulbs[2].text = scoreCharList.Length - 5 >= 0 ? scoreCharList[scoreCharList.Length - 5].ToString() : "0";
        scoreBulbs[3].text = scoreCharList.Length - 4 >= 0 ? scoreCharList[scoreCharList.Length - 4].ToString() : "0";
        scoreBulbs[4].text = scoreCharList.Length - 3 >= 0 ? scoreCharList[scoreCharList.Length - 3].ToString() : "0";
        scoreBulbs[5].text = scoreCharList.Length - 2 >= 0 ? scoreCharList[scoreCharList.Length - 2].ToString() : "0";
        scoreBulbs[6].text = scoreCharList.Length - 1 >= 0 ? scoreCharList[scoreCharList.Length - 1].ToString() : "0";
    }

    public void SetWavesUI(int currentWave)
    {
        string waves = currentWave.ToString();
        var waveCharList = waves.ToCharArray();
        if (waveBulbs.Length < waveCharList.Length)
        {
            waveBulbs[0].text = "9";
            waveBulbs[1].text = "9";
            waveBulbs[2].text = "9";
            return;
        }
        waveBulbs[0].text = waveCharList.Length - 3 >= 0 ? waveCharList[waveCharList.Length - 3].ToString() : "0";
        waveBulbs[1].text = waveCharList.Length - 2 >= 0 ? waveCharList[waveCharList.Length - 2].ToString() : "0";
        waveBulbs[2].text = waveCharList.Length - 1 >= 0 ? waveCharList[waveCharList.Length - 1].ToString() : "0";
    }

    public void SetWaveSliderIU(float fillAmount)
    {
        waveSliderImage.fillAmount = fillAmount;
    }

    public void CheckHealth(float currentHealth, float maxHealth)
    {
        bobbleHeadAnimator.SetTrigger("Shake");

        string health = currentHealth.ToString();
        var healthList = health.ToCharArray();

        healthNixieTexts[0].text = healthList.Length - 3 >= 0 ? healthList[healthList.Length - 3].ToString() : "0";
        healthNixieTexts[1].text = healthList.Length - 2 >= 0 ? healthList[healthList.Length - 2].ToString() : "0";
        healthNixieTexts[2].text = healthList.Length - 1 >= 0 ? healthList[healthList.Length - 1].ToString() : "0";

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
