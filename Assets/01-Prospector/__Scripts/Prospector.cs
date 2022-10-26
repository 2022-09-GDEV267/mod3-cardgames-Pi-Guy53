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
	public List<CardProspector> drawPile;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;

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

}