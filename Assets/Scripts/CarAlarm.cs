using UnityEngine;
using System.Collections;

public class CarAlarmTrigger : MonoBehaviour
{
    [Header("Lights")]
    public Light leftLight;
    public Light rightLight;
    public Light brakelightLeft;
    public Light brakelightRight;

    [Header("Audio")]
    public AudioSource caralarm;

    [Header("Behavior")]
    public bool alternatePattern = true;
    [Range(0.01f, 1.0f)] public float fadeTime = 0.15f;
    [Range(0.0f, 1.0f)] public float holdTime = 0.05f;
    public float headlightMaxIntensity = 2f;
    public float brakeMaxIntensity = 2f;

    private bool flashing;
    private bool alarmActive;

    private float lH0, rH0, lB0, rB0;

    void Awake()
    {
        if (leftLight) lH0 = leftLight.intensity;
        if (rightLight) rH0 = rightLight.intensity;
        if (brakelightLeft) lB0 = brakelightLeft.intensity;
        if (brakelightRight) rB0 = brakelightRight.intensity;
        SetHeadlightsIntensity(0f);
        SetBrakesIntensity(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        alarmActive = !alarmActive;
        if (alarmActive)
        {
            if (caralarm) caralarm.Play();
            StartCoroutine(FlashLightsSmooth());
        }
        else
        {
            if (caralarm) caralarm.Stop();
            flashing = false;
            StopAllCoroutines();
            SetHeadlightsIntensity(0f);
            SetBrakesIntensity(0f);
        }
    }

    private IEnumerator FlashLightsSmooth()
    {
        flashing = true;
        EnableAll(true);
        while (flashing)
        {
            if (alternatePattern)
            {
                yield return FadePair(headTarget: headlightMaxIntensity, brakeTarget: 0f, duration: fadeTime);
                if (!flashing) break;
                if (holdTime > 0f) yield return new WaitForSeconds(holdTime);
                yield return FadePair(headTarget: 0f, brakeTarget: brakeMaxIntensity, duration: fadeTime);
                if (!flashing) break;
                if (holdTime > 0f) yield return new WaitForSeconds(holdTime);
            }
            else
            {
                yield return FadePair(headTarget: headlightMaxIntensity, brakeTarget: brakeMaxIntensity, duration: fadeTime);
                if (!flashing) break;
                if (holdTime > 0f) yield return new WaitForSeconds(holdTime);
                yield return FadePair(headTarget: 0f, brakeTarget: 0f, duration: fadeTime);
                if (!flashing) break;
                if (holdTime > 0f) yield return new WaitForSeconds(holdTime);
            }
        }
        SetHeadlightsIntensity(0f);
        SetBrakesIntensity(0f);
    }

    private void EnableAll(bool on)
    {
        if (leftLight) leftLight.enabled = on;
        if (rightLight) rightLight.enabled = on;
        if (brakelightLeft) brakelightLeft.enabled = on;
        if (brakelightRight) brakelightRight.enabled = on;
    }

    private void SetHeadlightsIntensity(float v)
    {
        if (leftLight) leftLight.intensity = v;
        if (rightLight) rightLight.intensity = v;
    }

    private void SetBrakesIntensity(float v)
    {
        if (brakelightLeft) brakelightLeft.intensity = v;
        if (brakelightRight) brakelightRight.intensity = v;
    }

    private IEnumerator FadePair(float headTarget, float brakeTarget, float duration)
    {
        float t = 0f;
        float h0 = leftLight ? leftLight.intensity : 0f;
        float h1 = rightLight ? rightLight.intensity : h0;
        float b0 = brakelightLeft ? brakelightLeft.intensity : 0f;
        float b1 = brakelightRight ? brakelightRight.intensity : b0;
        while (t < duration)
        {
            if (!flashing) yield break;
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float hi = Mathf.Lerp(h0, headTarget, k);
            float bi = Mathf.Lerp(b0, brakeTarget, k);
            SetHeadlightsIntensity(hi);
            SetBrakesIntensity(bi);
            yield return null;
        }
        SetHeadlightsIntensity(headTarget);
        SetBrakesIntensity(brakeTarget);
    }
}
