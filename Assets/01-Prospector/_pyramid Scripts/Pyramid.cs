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
}