using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierMonitor : MonoBehaviour
{
    public TMP_Text multiplierText;

    public GameObject resetScreen;

    public IEnumerator resetAnimations()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
