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

    public override void OnMouseUpAsButton()
    {
        Pyramid.S.cardClicked(this);
    }
}