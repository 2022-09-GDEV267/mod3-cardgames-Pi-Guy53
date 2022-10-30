using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
	static public Pyramid S;

	[Header("Set in Inspector")]
	public TextAsset deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3, yOffest = -2.5f;
	public Vector3 layoutCenter;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardPyramid> drawPile;

	public Transform layoutAnchor;
	public CardPyramid target;
	public List<CardPyramid> pyramidLayout;
	public List<CardPyramid> discardPile;

    private void Awake()
    {
		S = this;
	}

    private void Start()
	{
		deck = GetComponent<Deck>();
		deck.InitDeck(deckXML.text);

		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);

		drawPile = ConvertListCardsToListCardPyramid(deck.cards);

		LayoutGame();
	}

	CardPyramid Draw()
	{
		CardPyramid cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
	}

	void LayoutGame()
    {
		if (layoutAnchor == null)
		{
			layoutAnchor = new GameObject("_LayoutAnchor").transform;
			layoutAnchor.transform.position = layoutCenter;
		}

		CardPyramid cp;

		foreach (SlotDef tSD in layout.slotDefs)
		{
			cp = Draw();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;

			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;

			cp.state = pCardState.pyramid;
			cp.SetSortingLayerName(tSD.layerName);

			pyramidLayout.Add(cp);
		}

		foreach (CardPyramid tCP in pyramidLayout)
		{
			foreach (int hid in tCP.slotDef.hiddenBy)
			{
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
			}
		}

		MoveToTarget(Draw());
		UpdateDrawPile();
	}

	CardPyramid FindCardByLayoutID(int layoutID)
	{
		foreach (CardPyramid tCP in pyramidLayout)
		{
			if (tCP.layoutID == layoutID)
			{
				return tCP;
			}
		}

		return null;
	}

	List<CardPyramid> ConvertListCardsToListCardPyramid(List<Card> CD1)
	{
		List<CardPyramid> CP1 = new List<CardPyramid>();
		CardPyramid tCP;

		foreach (Card tCD in CD1)
		{
			tCP = tCD as CardPyramid;
			CP1.Add(tCP);
		}

		return CP1;
	}

	void MoveToDiscard(CardPyramid cd)
	{
		cd.state = pCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;

		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + .5f);
		cd.faceUp = true;

		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}

	void MoveToTarget(CardPyramid cd)
	{
		if (target != null)
		{
			MoveToDiscard(target);
		}
		target = cd;
		cd.state = pCardState.target;

		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);

		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}

	void UpdateDrawPile()
	{
		CardPyramid cd;

		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x), layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), -layout.drawPile.layerID + .1f * i);

			cd.faceUp = false;
			cd.state = pCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}
}