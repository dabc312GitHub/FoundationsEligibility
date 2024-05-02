using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	TicTacToeAI _ai;

	[SerializeField]
	private int _myCoordX = 0;
	[SerializeField]
	private int _myCoordY = 0;

	[SerializeField]
	private bool canClick;

	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}


	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEndabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
	}

	private void SetInputEndabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
		canClick = true;
	}

	// private bool bPlayerWaitForAI = false;
	private void OnMouseDown()
	{
		if(canClick){
			SetCannotClick();
			_ai.PlayerSelects(_myCoordX, _myCoordY);
			_ai.FindSpaceBlankForAI();
		}
		print("aa");
	}

	public void SetCannotClick()
	{
		canClick = false;
	}
	public bool GetClickState()
	{
		return canClick;
	}

	public void SetCoord_XY(int coordX, int coordY)
	{
		_myCoordX = coordX;
		_myCoordY = coordY;
	}

	public Vector2 GetCoord_XY()
	{
		return new Vector2(_myCoordX, _myCoordY);
	}
}
