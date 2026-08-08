using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TimeAdjustmentReason
{
    NONE,
    DAMAGE,
    HEAL,
    COUNTDOWN,
}

public class BarUI : MonoBehaviour
{
    [SerializeField] RectTransform rootRc;
    [SerializeField] Image frontImage;
    [SerializeField] Image rearImage;
    [SerializeField] Image addedTimeVfxImage; //Optional, used for VFX

    [SerializeField] TextMeshProUGUI timerTxt;
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float damageAnimTime = 0.2f;
    [SerializeField] float damageAnimScale = 0.5f;

    [SerializeField] bool testTimer = false;
    [SerializeField] TimeAdjustmentReason timerTestMode = TimeAdjustmentReason.NONE;

    float maxWidth = 0.0f;
    float currTime = -1;
    float maxTime = -1;

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

    //Call this when you want to change the current health/time
    public void SetRemainingTime(in Character character, TimeAdjustmentReason optReason = TimeAdjustmentReason.NONE /*Used to show VFX on the bar*/)
    {
        SetRemainingTime(character.currentHealth, character.maxHealth, optReason);
    }


    public void SetRemainingTime(float time, float totalTime, TimeAdjustmentReason optReason = TimeAdjustmentReason.NONE /*Used to show VFX on the bar*/)
    {
        // prevent division by 0
        totalTime = Mathf.Max(1.0f, totalTime);

        if (timerTxt != null)
        {
            timerTxt.SetText(Main.FormatTime(time));
        }

        if (time < currTime && addedTimeVfxImage != null)
            addedTimeVfxImage.enabled = false;

        if (time > currTime && optReason == TimeAdjustmentReason.HEAL && addedTimeVfxImage != null) //Gained health from a perk
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
            if (damageVfxCr == null)
            {
                damageVfxCr = StartCoroutine(DamageVFX());
            }
        }

        currTime = time;
        maxTime = totalTime;
    }

    float GetWidthForTime(float time, float totalTime)
    {
        float nrm = 1.0f - (time / totalTime);
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
            case TimeAdjustmentReason.HEAL:
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
            pivot.y += damageAnimScale * Random.Range(-1.0f, 1.0f) * Time.deltaTime;
            pivot.x += damageAnimScale * Random.Range(-1.0f, 1.0f) * Time.deltaTime;
            rootRc.pivot = pivot;
            vfxTime -= Time.deltaTime;

            yield return null;
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
        Vector2 currentSizeDelta = frontImage.rectTransform.sizeDelta;
        float newSizeDeltaX = Mathf.Lerp(currentSizeDelta.x, targetWidth, moveSpeed * Time.deltaTime);

        if (!Mathf.Approximately(currentSizeDelta.x, newSizeDeltaX))
        {
            frontImage.rectTransform.sizeDelta = new Vector2(newSizeDeltaX, currentSizeDelta.y);
        }

        if (addedTimeVfxImage != null && addedTimeVfxImage.enabled)
        {
            if (Mathf.Approximately(newSizeDeltaX, targetWidth))
            {
                addedTimeVfxImage.enabled = false;
            }
        }
    }
}
