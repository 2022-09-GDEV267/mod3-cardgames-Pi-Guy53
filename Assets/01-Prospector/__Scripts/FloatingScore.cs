using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eFSState {  idle, pre, active, post}

public class FloatingScore : MonoBehaviour
{
    public eFSState state = eFSState.idle;

    [SerializeField]
    protected int _score = 0;
    public string scoreString;

    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreString = _score.ToString("NO");
            GetComponent<Text>().text = scoreString;
        }
    }

    public List<Vector2> bezierPts;
    public List<float> fontSizes;
    public float timeStart = -1, timeDurration = 1;
    public string easingCurve = Easing.InOut;

    public GameObject reportFinishTo = null;
    private RectTransform rectTrans;
    private Text txt;

    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;

        txt = GetComponent<Text>();
        bezierPts = new List<Vector2>(ePts);

        if(ePts.Count == 1)
        {
            transform.position = ePts[0];
            return;
        }

        if(eTimeS == 0)
        {
            eTimeS = Time.time;
        }
        timeStart = eTimeS;
        timeDurration = eTimeD;

        state = eFSState.pre;
    }

    public void FSCallBack(FloatingScore fs)
    {
        score += fs.score;
    }

    private void Update()
    {
        if(state == eFSState.idle)
        {
            return;
        }

        float u = (Time.time - timeStart) / timeDurration;
        float uc = Easing.Ease(u, easingCurve);

        if(u < 0)
        {
            state = eFSState.pre;
            txt.enabled = false;
        }
        else
        {
            if(u>=1)
            {
                uc = 1;
                state = eFSState.post;
                if(reportFinishTo != null)
                {
                    reportFinishTo.SendMessage("FSCallBack", this);
                    Destroy(gameObject);
                }
                else
                {
                    state = eFSState.idle;
                }
            }
            else
            {
                state = eFSState.active;
                txt.enabled = true;
            }

            Vector2 pos = Utils.Bezier(uc, bezierPts);

            rectTrans.anchorMin = rectTrans.anchorMax = pos;
            if(fontSizes != null && fontSizes.Count>0)
            {
                int size = Mathf.RoundToInt(Utils.Bezier(uc, fontSizes));
                GetComponent<Text>().fontSize = size;
            }
        }
    }
}