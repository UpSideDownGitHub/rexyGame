using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ImplosionPP : MonoBehaviour
{
    private Volume volume;

    public ChromaticAberration chromatic;
    public DepthOfField depth;

    private void Start()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet(out chromatic);
        volume.profile.TryGet(out depth);

        StartCoroutine(TriggerPP());
    }

    private IEnumerator TriggerPP()
    {
        int iterations = 40;

        yield return new WaitForSeconds(0.16f);
        chromatic.active = true;
        depth.active = true;

        while (iterations > 0)
        {
            iterations--;
            chromatic.intensity.value -= 0.025f;

            depth.gaussianMaxRadius.value -= 0.1f;

            yield return new WaitForSeconds(0.025f);
        }

        chromatic.active = false;
        depth.active = false;
    }
}