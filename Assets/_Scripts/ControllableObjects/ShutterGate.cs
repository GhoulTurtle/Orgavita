using System;
using System.Collections;
using UnityEngine;

public class ShutterGate : ControllableObject{
    [Header("Shutter Gate Variables")]
    [SerializeField] private Vector3 travelPosition;
    [SerializeField] private float travelSpeed;

    private Vector3 startPos;
    private Vector3 endPos;
    private float current;
    private int target;

    private const float SNAP_DISTANCE = 0.01f;

    private void Start() {
        startPos = transform.position;
        endPos = transform.position + travelPosition;
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public override void Activate(){
        if(isActivated) return;

        StopAllCoroutines();
        StartCoroutine(GateMovingCorutine(true));

        isActivated = true;
        OnActivate?.Invoke(this, EventArgs.Empty);
    }

    public override void Deactivate(){
        if(!isActivated) return;

        StopAllCoroutines();
        StartCoroutine(GateMovingCorutine(false));

        isActivated = false;
        OnDeactivate?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator GateMovingCorutine(bool activate){
        target = activate ? 1 : 0;
        Vector3 currentTarget = activate ? endPos : startPos;

        while(Vector3.Distance(transform.position, currentTarget) > SNAP_DISTANCE){
            current = Mathf.MoveTowards(current, target, travelSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(startPos, endPos, current);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + travelPosition, transform.localScale);
    }
}
