//
//  GameState.h
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

enum PXGameState
{
	gsStarting = 0,
	gsHandStarting = 100,
	gsTurnStarting = 250,
	gsPickingCard = 300,
	gsPlaying = 500,
	gsTurnEnding = 550,
	gsHandEnding = 600,
	gsEnding = 700,
	gsGameOver = 800,
};

const char *GameStateToString(enum PXGameState gameState);
