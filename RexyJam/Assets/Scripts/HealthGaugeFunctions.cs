using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGaugeFunctions : MonoBehaviour
{
    public Animator bulbAnimator;
    public GameObject gaugeArrow;

    public void CheckHealth(float currentHealth, float maxHealth)
    {
        if (currentHealth / maxHealth <= 0.2f)
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

    private IEnumerator ChangeGauge()
    {
        gaugeArrow.transform.eulerAngles = new Vector2();
        yield return null;
    }
}
