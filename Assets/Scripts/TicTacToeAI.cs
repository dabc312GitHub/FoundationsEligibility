using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState = new TicTacToeState[3,3];

	[SerializeField]
	private bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.cross;
	TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	private void Start()
	{
		bool bTempPlayerState = playerState == TicTacToeState.circle || playerState == TicTacToeState.none;
		playerState = bTempPlayerState ? TicTacToeState.circle : TicTacToeState.cross;
		aiState = bTempPlayerState ? TicTacToeState.cross : TicTacToeState.circle;

		for(int i = 0; i < _gridSize; i++){
			for(int j = 0; j < _gridSize; j++){
				boardState[i, j] = TicTacToeState.none;
			}
		}
	}

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, playerState);
		print("player selects");
	}

	public void AiSelects(int coordX, int coordY){
		print("Coord: "+coordX+", "+coordY);
		SetVisual(coordX, coordY, aiState);
		print("AI selects");
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		boardState[coordX, coordY] = targetState;
		GameObject prefabTarget = targetState == TicTacToeState.circle ? _oPrefab : _xPrefab;
		Instantiate(
			prefabTarget, 
			_triggers[coordX, coordY].transform.position, 
			Quaternion.identity
		);
	}

	public void FindSpaceBlankForAI()
	{
		// Version 0: Naive Search
		// NaiveSearchTest();

		// version 1: Random Search
		Vector2 bestMove = findBestMove(boardState);
		print(bestMove[0]+", "+bestMove[1]);
		if (bestMove[0] < 0 || bestMove[1] < 0) return;
		AiSelects((int)bestMove[0], (int)bestMove[1]);
	}

	private void NaiveSearchTest() {
		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				if(boardState[i, j] == TicTacToeState.none)
				{
					AiSelects(i, j);
					return;
				}
			}
		}
	}

	bool isMovesLeft(TicTacToeState [,]board) 
	{ 
		for (int i = 0; i < 3; i++) 
			for (int j = 0; j < 3; j++) 
				if (board[i, j] == TicTacToeState.none) 
					return true; 
		return false; 
	} 

	int evaluate(TicTacToeState [,]board) 
	{ 
		// Checking for Rows for X or O victory. 
		for (int row = 0; row < 3; row++) 
		{ 
			if (board[row, 0] == board[row, 1] && 
				board[row, 1] == board[row, 2]) 
			{ 
				if (board[row, 0] == playerState) 
					return +10; 
				else if (board[row, 0] == aiState) 
					return -10; 
			} 
		} 
	
		// Checking for Columns for X or O victory. 
		for (int col = 0; col < 3; col++) 
		{ 
			if (board[0, col] == board[1, col] && 
				board[1, col] == board[2, col]) 
			{ 
				if (board[0, col] == playerState) 
					return +10; 
	
				else if (board[0, col] == aiState) 
					return -10; 
			} 
		} 
	
		// Checking for Diagonals for X or O victory. 
		if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2]) 
		{ 
			if (board[0, 0] == playerState) 
				return +10; 
			else if (board[0, 0] == aiState) 
				return -10; 
		} 
	
		if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0]) 
		{ 
			if (board[0, 2] == playerState) 
				return +10; 
			else if (board[0, 2] == aiState) 
				return -10; 
		} 
	
		// Else if none of them have won then return 0 
		return 0; 
	} 

	private int minimax(TicTacToeState [,]board, int depth, bool isMax) 
	{ 
		int score = evaluate(board); 
	
		// If Maximizer has won the game  
		// return his/her evaluated score 
		if (score == 10) 
			return score; 
	
		// If Minimizer has won the game  
		// return his/her evaluated score 
		if (score == -10) 
			return score; 
	
		// If there are no more moves and  
		// no winner then it is a tie 
		if (isMovesLeft(board) == false) 
			return 0; 
	
		// If this maximizer's move 
		if (isMax) 
		{ 
			int best = -1000; 
	
			// Traverse all cells 
			for (int i = 0; i < 3; i++) 
			{ 
				for (int j = 0; j < 3; j++) 
				{ 
					// Check if cell is empty 
					if (board[i, j] == TicTacToeState.none) 
					{ 
						// Make the move 
						board[i, j] = playerState; 
	
						// Call minimax recursively and choose 
						// the maximum value 
						best = Math.Max(best, minimax(board, depth + 1, !isMax)); 
	
						// Undo the move 
						board[i, j] = TicTacToeState.none; 
					} 
				} 
			} 
			return best; 
		} 
	
		// If this minimizer's move 
		else
		{ 
			int best = 1000; 
	
			// Traverse all cells 
			for (int i = 0; i < 3; i++) 
			{ 
				for (int j = 0; j < 3; j++) 
				{ 
					// Check if cell is empty 
					if (board[i, j] == TicTacToeState.none)
					{ 
						// Make the move 
						board[i, j] = aiState; 
	
						// Call minimax recursively and choose 
						// the minimum value 
						best = Math.Min(best, minimax(board, depth + 1, !isMax)); 
	
						// Undo the move 
						board[i, j] = TicTacToeState.none; 
					} 
				} 
			} 
			return best; 
		} 
	} 

	Vector2 findBestMove(TicTacToeState [,]board) 
	{ 
		int bestVal = -1000; 
		Vector2 bestMove = new Vector2(); 
		bestMove[0] = -1;
		bestMove[1] = -1;
		// bestMove.SetCoord_XY(-1, -1);
		// bestMove.row = -1; 
		// bestMove.col = -1; 
	
		// Traverse all cells, evaluate minimax function  
		// for all empty cells. And return the cell  
		// with optimal value. 
		for (int i = 0; i < 3; i++) 
		{ 
			for (int j = 0; j < 3; j++) 
			{ 
				// Check if cell is empty 
				if (board[i, j] == TicTacToeState.none) 
				{ 
					// Make the move 
					board[i, j] = playerState; 
	
					// compute evaluation function for this 
					// move. 
// minimaxAlphaBeta(	int depth, int nodeIndex, 
// 					bool maximizingPlayer,
// 					int []values, int alpha,
// 					int beta)

// 					int moveVal = minimaxAlphaBeta(board, 0, false); 
					int moveVal = minimax(board, 0, false); 
	
					// Undo the move 
					board[i, j] = TicTacToeState.none; 
	
					// If the value of the current move is 
					// more than the best value, then update 
					// best/ 
					if (moveVal > bestVal) 
					{ 
						bestMove[0] = i;
						bestMove[1] = j;
						// bestMove.SetCoord_XY(i, j);
						// bestMove.row = i; 
						// bestMove.col = j; 
						bestVal = moveVal; 
					} 
				} 
			} 
		} 
	
		// Console.Write("The value of the best Move " +  
		// 					"is : {0}\n\n", bestVal); 
	
		return bestMove; 
	} 

	// Initial values of 
	// Alpha and Beta
	static int MAX = 1000;
	static int MIN = -1000;
	
	// Returns optimal value for
	// current player (Initially called
	// for root and maximizer)
	private int minimaxAlphaBeta(int depth, int nodeIndex, 
					bool maximizingPlayer,
					int []values, int alpha,
					int beta)
	{
		// Terminating condition. i.e 
		// leaf node is reached
		if (depth == 3)
			return values[nodeIndex];
	
		if (maximizingPlayer)
		{
			int best = MIN;
	
			// Recur for left and
			// right children
			for (int i = 0; i < 2; i++)
			{
				int val = minimaxAlphaBeta(depth + 1, nodeIndex * 2 + i, false, values, alpha, beta);
				best = Math.Max(best, val);
				alpha = Math.Max(alpha, best);
	
				// Alpha Beta Pruning
				if (beta <= alpha)
					break;
			}
			return best;
		}
		else
		{
			int best = MAX;
	
			// Recur for left and
			// right children
			for (int i = 0; i < 2; i++)
			{
				
				int val = minimaxAlphaBeta(depth + 1, nodeIndex * 2 + i, true, values, alpha, beta);
				best = Math.Min(best, val);
				beta = Math.Min(beta, best);
	
				// Alpha Beta Pruning
				if (beta <= alpha)
					break;
			}
			return best;
		}
		// https://github.com/dabc312GitHub/IA_projects/blob/master/lab_3/minMaxAlfaBeta.py
	}
}
