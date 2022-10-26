using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour
{
	static public Prospector S;

	[Header("Set in Inspector")]
	public TextAsset deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3, yOffest = -2.5f;
	public Vector3 layoutCenter;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardProspector> drawPile;

	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;

	void Awake()
	{
		S = this;
	}

	void Start()
	{
		deck = GetComponent<Deck>();
		deck.InitDeck(deckXML.text);

		Deck.Shuffle(ref deck.cards);

		/*Card c;
		for (int cNum = 0; cNum < deck.cards.Count; cNum++)
		{
			c = deck.cards[cNum];
			c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
		} */

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);

		drawPile = ConvertListCardsToListCardProspectors(deck.cards);

		LayoutGame();
	}

	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> CD1)
	{
		List<CardProspector> CP1 = new List<CardProspector>();
		CardProspector tCP;

		foreach (Card tCD in CD1)
		{
			tCP = tCD as CardProspector;
			CP1.Add(tCP);
		}

		return CP1;
    }

	CardProspector Draw()
    {
		CardProspector cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
    }

	void LayoutGame()
    {
		if(layoutAnchor == null)
        {
			layoutAnchor = new GameObject("_LayoutAnchor").transform;
			layoutAnchor.transform.position = layoutCenter;
        }

		CardProspector cp;

		foreach (SlotDef tSD in layout.slotDefs)
        {
			cp = Draw();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;

			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;

			cp.state = eCardState.tableau;
			cp.SetSortingLayerName(tSD.layerName);

			tableau.Add(cp);
        }
    }

}