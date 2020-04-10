//
//  CardStackView.h
//  phazex
//
//  Created by toddha on 11/14/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardCollection.h"
#import "CardView.h"
#import "Card.h"

@class PXCardStackArtist;
@class CardStackView;
@protocol CardStackViewDelegate <NSObject>
- (void)cardStackView:(CardStackView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardView:(CardView *)cardView;
@end

@interface CardStackView : NSView
{
	PXCardStackArtist *_artist;
	id<CardStackViewDelegate> _delegate;
	NSTrackingArea *_trackingArea;
}

@property (readwrite, assign) id<CardStackViewDelegate> delegate;
@property (readwrite, retain) CardView *selectedCardView;

//- (void)addCard:(PXCard *)card;
- (CardView *)cardViewAtPoint:(NSPoint)point;

- (void)setEnabled:(BOOL)enabled;
- (void)setHovers:(BOOL)hovers;
- (void)setCards:(PXCardCollection *)cards;
- (void)setStacked:(BOOL)stacked;
- (void)setFanEffect:(BOOL)fanEffect;

@end

