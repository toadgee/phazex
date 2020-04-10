//
//  PlayerStatusView.m
//  phazex
//
//  Created by toddha on 12/12/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "PlayerStatusView_ios.h"
#import "Player.h"
#import "PlayerStatusArtist.h"

@interface PXPlayerStatusView () <PXPlayerStatusArtistDelegate>
{
@public
	PXPlayerStatusArtist *_artist;
}
@end

void PXPlayerStatusViewCommonInit(PXPlayerStatusView *view);
void PXPlayerStatusViewCommonInit(PXPlayerStatusView *view)
{
	view->_artist = [[PXPlayerStatusArtist alloc] init];
	[view->_artist setDelegate:view];
}

@implementation PXPlayerStatusView

- (instancetype)initWithCoder:(NSCoder *)decoder
{
	self = [super initWithCoder:decoder];
	if (self)
	{
		PXPlayerStatusViewCommonInit(self);
    }

    return self;
}

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
	if (self)
	{
		PXPlayerStatusViewCommonInit(self);
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
	[self setNeedsDisplay];
}

- (void)setPlayers:(NSArray<PXPlayer *> *)players
{
	[_artist setPlayers:players];
}

- (void)drawRect:(CGRect)rect
{
	[_artist drawRect:rect];
}

@end
