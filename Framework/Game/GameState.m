#include "GameState.h"

const char *GameStateToString(enum PXGameState gameState)
{
	switch (gameState)
	{
		case gsStarting: return "gsStarting";
		case gsHandStarting: return "gsHandStarting";
		case gsTurnStarting: return "gsTurnStarting";
		case gsPickingCard: return "gsPickingCard";
		case gsPlaying: return "gsPlaying";
		case gsTurnEnding: return "gsTurnEnding";
		case gsHandEnding: return "gsHandEnding";
		case gsEnding: return "gsEnding";
		case gsGameOver: return "gsGameOver";
	}
}
