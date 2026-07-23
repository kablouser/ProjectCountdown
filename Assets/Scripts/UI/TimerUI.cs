using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public enum TimeAdjustmentReason
{
    NONE,
    DAMAGE,
    PERK
}

public class TimerUI : MonoBehaviour
{
    [SerializeField] RectTransform rootRc;
    [SerializeField] Image frontImage;
    [SerializeField] Image rearImage;
    [SerializeField] Image addedTimeVfxImage;

    [SerializeField] TextMeshProUGUI timerTxt;
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float damageAnimTime = 0.25f;

    [SerializeField] bool testTimer = false;
    [SerializeField] TimeAdjustmentReason timerTestMode = TimeAdjustmentReason.NONE;

    float maxWidth = 0.0f;
    int currTime = -1;
    int maxTime = -1;

    Coroutine damageVfxCr = null;

    private void Awake()
    {
        maxWidth = frontImage.rectTransform.sizeDelta.x;
    }

    void Start()
    {
        if(testTimer)
            StartCoroutine(TimerTest());
    }

    public void SetRemainingTime(int time, int totalTime, TimeAdjustmentReason optReason = TimeAdjustmentReason.NONE /*Used to show VFX on the bar*/)
    {
        timerTxt.SetText($"{time}");

        if (time > currTime && optReason == TimeAdjustmentReason.PERK) //Gained health from a perk
        {
            //Show a leading bar that the actual health bar catches up to

            float desiredWidth = GetWidthForTime(time, totalTime);
            Vector2 sizeDelta = addedTimeVfxImage.rectTransform.sizeDelta;
            sizeDelta.x = desiredWidth;

            addedTimeVfxImage.rectTransform.sizeDelta = sizeDelta;
            addedTimeVfxImage.enabled = true;
        }
        else if (time < currTime && optReason == TimeAdjustmentReason.DAMAGE) //Lost health from an enemy
        {
            //Shake the UI

            addedTimeVfxImage.enabled = false;

            if (damageVfxCr == null)
            {
                damageVfxCr = StartCoroutine(DamageVFX());
            }
        }

        currTime = time;
        maxTime = totalTime;
    }

    float GetWidthForTime(int time, int totalTime)
    {
        float nrm = 1.0f - ((time * 1.0f) / (totalTime * 1.0f));
        return nrm * maxWidth;
    }

    IEnumerator TimerTest()
    {
        int debugMaxTime = 100;

        switch (timerTestMode)
        {
            case TimeAdjustmentReason.DAMAGE:
                {
                    int time = debugMaxTime;

                    while (time > 0)
                    {
                        SetRemainingTime(time, debugMaxTime, timerTestMode);
                        yield return new WaitForSeconds(1.0f);
                        time--;
                    }
                    break;
                }
            case TimeAdjustmentReason.PERK:
                {
                    int time = 0;
                    while (time < debugMaxTime)
                    {
                        SetRemainingTime(time, debugMaxTime, timerTestMode);
                        yield return new WaitForSeconds(1.0f);
                        time++;
                    }
                    break;
                }
            case TimeAdjustmentReason.NONE:
                {
                    int time = 0;
                    while (time < debugMaxTime)
                    {
                        SetRemainingTime(time, debugMaxTime);
                        yield return new WaitForSeconds(1.0f);
                        time++;
                    }
                    break;
                }
        }
    }

    IEnumerator DamageVFX()
    {
        //Shake the timer 

        float vfxTime = damageAnimTime;
        Vector3 startPos = transform.position;

        Vector2 pivot = rootRc.pivot;
        Vector2 startPivot = pivot;

        while (vfxTime >= 0.0f)
        {
            pivot.y += (UnityEngine.Random.Range(-1.0f, 1.0f));
            pivot.x += (UnityEngine.Random.Range(-1.0f, 1.0f));
            rootRc.pivot = pivot;
            vfxTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rootRc.pivot = startPivot;
        damageVfxCr = null;
    }

    private void Update()
    {
        if (currTime == -1 || maxTime == -1)
        {
            return;
        }

        float targetWidth = GetWidthForTime(currTime, maxTime);
        Vector2 sizeDelta = frontImage.rectTransform.sizeDelta;
        sizeDelta.x = Mathf.Lerp(sizeDelta.x, targetWidth, moveSpeed * Time.deltaTime);

        frontImage.rectTransform.sizeDelta = sizeDelta;

        if (addedTimeVfxImage.enabled)
        {
            if (Mathf.Approximately(sizeDelta.x, targetWidth))
            {
                addedTimeVfxImage.enabled = false;
            }
        }
    }
}
