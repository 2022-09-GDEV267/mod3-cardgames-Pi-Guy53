using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState {  drawpile, tableau, target, discard}

public class CardProspector : Card
{
    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int layoutID;
    public SlotDef slotDef;

    public bool isGold;

    public override void OnMouseUpAsButton()
    {
        Prospector.S.CardClicked(this);
    }
}