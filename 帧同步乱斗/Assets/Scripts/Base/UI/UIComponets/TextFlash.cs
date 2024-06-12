using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFlash : UIEffect
{
    public TextMeshProUGUI text;
    public float loopTime = 1.5f;
    [Tooltip("纵坐标0到1，横坐标0到1，表示随着时间的透明度变化")]
    public AnimationCurve flashCurve;

    public float time = 0;


    private void Reset()
    {
        text = GetComponent<TextMeshProUGUI>();
        flashCurve = new();
        flashCurve.AddKey(new Keyframe(0, 0));
        flashCurve.AddKey(new Keyframe(0.5f, 1f));
        flashCurve.AddKey(new Keyframe(1f, 0f));
    }

    public override void Start()
    {
        base.Start();
        time = 0;
    }

    public override void Update()
    {
        base.Update();
        time += TimeMgr.Instance.realTimeDelta;
        time = time % loopTime;
        float alpha = flashCurve.Evaluate(time / loopTime);
        text.alpha = alpha;
    }
}
