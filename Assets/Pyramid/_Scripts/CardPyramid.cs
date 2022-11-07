using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum pCardState { drawpile, pyramid, target, discard }

public class CardPyramid : Card
{
    public pCardState state = pCardState.drawpile;
    public List<CardPyramid> hiddenBy = new List<CardPyramid>();
    public int layoutID;
    public SlotDef slotDef;

    public bool isHidden;

    private Vector3 moveTarget;
    private bool isMoving;

    public override void OnMouseUpAsButton()
    {
        Pyramid.S.cardClicked(this);
    }

    private void Update()
    {
        if(isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, moveTarget, .1f);

            if(Vector2.Distance(transform.position, moveTarget) < .1f)
            {
                isMoving = false;
            }
        }
    }

    public void moveTo(Vector3 target)
    {
        moveTarget = target;
        isMoving = true;
    }
}