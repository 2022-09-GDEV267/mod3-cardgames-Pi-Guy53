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

	public Vector2 fsPosMid = new Vector2(.5f, .9f);
	public Vector2 fsPosrun = new Vector2(.5f, .75f);
	public Vector2 fsPosMid2 = new Vector2(.4f, 1f);
	public Vector2 fsPosEnd = new Vector2(.5f, .95f);

	public Text gameOverTxt, roundResultTxt, highScoreTxt;

	public FloatingScore fsRun;

	public float reloadDelay = 2;

	public Sprite goldBack, goldFront;

	void Awake()
	{
		S = this;
		SetUpUIText();
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

		ScoreBoard.s.score = ScoreManager.SCORE;

		LayoutGame();

		/*
		 * To Display as a face up grid of cards
		 * 
		CardProspector cp;
		for(int i = 0; i < 52; i++)
		{
			cp = Draw();
			cp.faceUp = true;
		} */
	}

	void SetUpUIText()
    {
		GameObject go = GameObject.Find("highScore");
		if(go!= null)
        {
			highScoreTxt = go.GetComponent<Text>();
        }
		int highScore = ScoreManager.HIGH_SCORE;
		string hScore = "high Score: " + Utils.AddCommasToNumber(highScore);
		go.GetComponent<Text>().text = hScore;

		go = GameObject.Find("GameOver");
        if (go != null)
		{
			gameOverTxt = go.GetComponent<Text>();
        }

		go = GameObject.Find("RoundResults");
        if( go != null)
		{
			roundResultTxt = go.GetComponent<Text>();
        }

		ShowResultsUI(false);
    }

	void ShowResultsUI(bool show)
    {
		gameOverTxt.gameObject.SetActive(show);
		roundResultTxt.gameObject.SetActive(show);
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
		if (layoutAnchor == null)
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

		foreach(CardProspector tCP in tableau)
        {
			foreach(int hid in tCP.slotDef.hiddenBy)
            {
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
            }
        }

		SetGoldCards();

		MoveToTarget(Draw());

		UpdateDrawPile();
	}

	void SetGoldCards()
    {
		for(int i = 0; i < tableau.Count; i++)
        {
			if (Random.value <= .10f)
			{
				tableau[i].GetComponent<CardProspector>().isGold = true;
				tableau[i].back.GetComponent<SpriteRenderer>().sprite = goldBack;
				tableau[i].GetComponent<SpriteRenderer>().sprite = goldFront;
			}
		}
    }

	CardProspector FindCardByLayoutID(int layoutID)
    {
		foreach(CardProspector tCP in tableau)
        {
			if(tCP.layoutID == layoutID)
            {
				return tCP;
            }
        }

		return null;
    }

	void SetTableauFaces()
    {
		foreach(CardProspector cd in tableau)
        {
			bool faceUp = true;

			foreach(CardProspector cover in cd.hiddenBy)
            {
				if(cover.state == eCardState.tableau)
                {
					faceUp = false;
                }
            }

			cd.faceUp = faceUp;
        }
    }

	void MoveToDiscard(CardProspector cd)
	{
		cd.state = eCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;

		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + .5f);
		cd.faceUp = true;

		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}

	void MoveToTarget(CardProspector cd)
    {
		if(target !=null)
        {
			MoveToDiscard(target);
        }
		target = cd;
		cd.state = eCardState.target;

		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);

		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
    }

	void UpdateDrawPile()
	{
		CardProspector cd;

		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x), layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), -layout.drawPile.layerID + .1f * i);

			cd.faceUp = false;
			cd.state = eCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}


	public void CardClicked(CardProspector cd)
	{
		switch (cd.state)
		{
			case eCardState.target:
				break;

			case eCardState.drawpile:
				MoveToDiscard(target);
				MoveToTarget(Draw());
				UpdateDrawPile();

				ScoreManager.EVENT(eScoreEvent.draw);
				FloatingScoreHandler(eScoreEvent.draw);
				break;

			case eCardState.tableau:
				bool validMatch = true;
				if (!cd.faceUp)
				{
					validMatch = false;
				}
				if (!AdjacentRank(cd, target))
				{
					validMatch = false;
				}
				if (!validMatch)
				{
					return;
				}

				tableau.Remove(cd);
				MoveToTarget(cd);
				SetTableauFaces();

				if (cd.isGold)
				{
					ScoreManager.EVENT(eScoreEvent.mineGold);
				}
				else
				{
					ScoreManager.EVENT(eScoreEvent.mine);
				}

				FloatingScoreHandler(eScoreEvent.mine);

				break;
		}

		CheckForGameOver();
	}

	void CheckForGameOver()
    {
		if (tableau.Count == 0)
        {
			GameOver(true);
			return;
        }

		foreach (CardProspector cd in tableau)
		{
			if (AdjacentRank(cd, target))
			{
				return;
			}
		}

		if (drawPile.Count > 0)
        {
			return;
        }

		GameOver(false);
    }

	void GameOver(bool won)
    {
		int score = ScoreManager.SCORE;
		if (fsRun != null) score += fsRun.score;

		if(won)
        {
			gameOverTxt.text = "Round Won";
			roundResultTxt.text = "you won this round!\nRound Score: " + score;

			ScoreManager.EVENT(eScoreEvent.gameWin);
			FloatingScoreHandler(eScoreEvent.gameWin);
		}
        else
        {
			gameOverTxt.text = "Game Over";
			if(ScoreManager.HIGH_SCORE <= score)
            {
				string str = "New High Score!\nHigh Score: " + score;
				roundResultTxt.text = str;
            }
            else
            {
				roundResultTxt.text = "Your final Score: " + score;
            }

			ScoreManager.EVENT(eScoreEvent.gameLoss);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}

		ShowResultsUI(true);

		Invoke("reloadLevel", reloadDelay);
    }

	void reloadLevel()
	{
		SceneManager.LoadScene("__Prospector");
	}

	public bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
		if(!c0.faceUp || !c1.faceUp)
        {
			return false;
        }

		if(Mathf.Abs(c0.rank - c1.rank) == 1)
        {
			return true;
        }

		if(c0.rank == 1 && c1.rank == 13)
        {
			return true;
        }
		if(c0.rank == 13 && c1.rank==1)
        {
			return true;
        }

		return false;
    }

	void FloatingScoreHandler(eScoreEvent evt)
    {
		List<Vector2> fsPts;
        switch(evt)
        {
			case eScoreEvent.draw:
			case eScoreEvent.gameWin:
			case eScoreEvent.gameLoss:
				if (fsRun != null)
                {
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosrun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);
					fsRun.reportFinishTo = ScoreBoard.s.gameObject;
					fsRun.Init(fsPts, 0, 1);
					fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
					fsRun.score *= ScoreManager.prevScoreMulti;
					fsRun = null;
				}
				break;

			case eScoreEvent.mine:
				FloatingScore fs;
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;

				fsPts = new List<Vector2>();
				fsPts.Add(p0);
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosrun);
				fs = ScoreBoard.s.CreateFloatingScore(ScoreManager.CHAIN, fsPts);

				fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
				if(fsRun == null)
                {
					fsRun = fs;
					fsRun.reportFinishTo = null;
                }
                else
                {
					fs.reportFinishTo = fsRun.gameObject;
                }
				break;

        }
    }
}