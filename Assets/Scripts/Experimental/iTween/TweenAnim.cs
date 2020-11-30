using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class TweenAnim : MonoBehaviour
{
    [SerializeField] private Transform tweenTarget;
    [SerializeField] private Transform positionTarget;
    [SerializeField] private float time = 3f;

    [SerializeField] private Vector3 originPos;
    [SerializeField] private float magn;
    [SerializeField] private float mult;

    private void Awake()
    {
        originPos = tweenTarget.position;
    }

    private void Update()
    {
        magn = (positionTarget.position - tweenTarget.position).magnitude;
        mult = Mathf.Clamp((positionTarget.position - originPos).magnitude / magn, 1f, 2f);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            LineExpand(tweenTarget.gameObject, Vector2.zero, Vector2.one);
        }
    }

    private void LineExpand(GameObject line, Vector2 position, Vector2 returnPosition)
    {
        // use speed instead of time for constant speed
        // time moves it in a given time
         iTween.MoveTo(line, iTween.Hash("name", "expandtween","position", positionTarget.position, "time", time / 2f / mult, "easetype", iTween.EaseType.linear, "oncomplete", "ShrinkTween", "onCompleteTarget", gameObject, "oncompleteparams", returnPosition));
        // iTween.MoveBy(tweenTarget.gameObject, iTween.Hash("name", "customcursortween", "amount", positionTarget.position, "time", time, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.linear, "oncomplete", "CancelNoobTween"));
        // iTween.MoveUpdate(tweenTarget.gameObject, iTween.Hash("position", positionTarget.position, "time", time)); // supposed to be called in update :D
    }

    private void ShrinkTween(Vector2 retPos)
    {
        Debug.Log("shrink " + " | " + retPos);
        iTween.MoveTo(tweenTarget.gameObject, iTween.Hash("name", "shrinktween", "position", originPos, "time", time / 2f, "easetype", iTween.EaseType.linear, "onCompleteTarget", gameObject));
    }

    private void CancelShrinkTween()
    {
        Debug.Log("cancel shrink");
        iTween.StopByName(tweenTarget.gameObject, "shrinktween", true);
    }
}
#pragma warning restore 0649