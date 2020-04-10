//
//  PlayerStatusView.m
//  phazex
//
//  Created by toddha on 12/12/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "PlayerStatusView_mac.h"
#import "Player.h"
#import "PlayerStatusArtist.h"

@interface PlayerStatusView () <PXPlayerStatusArtistDelegate>
{
	PXPlayerStatusArtist *_artist;
}
@end

@implementation PlayerStatusView

- (id)initWithFrame:(NSRect)frame
{
    self = [super initWithFrame:frame];
    
	if (self)
	{
		_artist = [[PXPlayerStatusArtist alloc] init];
		[_artist setDelegate:self];
    }

    return self;
}

-(void)dealloc
{
	[_artist release];
	[super dealloc];
}

- (void)playerStatusArtistsNeedsDisplay:(PXPlayerStatusArtist *)artist
{
	[self setNeedsDisplay:YES];
}

- (void)drawRect:(NSRect)rect
{
	[_artist drawRect:rect];
}

- (void)setPlayers:(NSArray<PXPlayer *> *)players
{
	[_artist setPlayers:players];
}

@end
