//
//  DeckView.m
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import "DeckView.h"

@implementation PXDeckView

- (void)touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] deckView:self beganTouches:touches];
}

- (void)touchesMoved:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] deckView:self movedTouches:touches];
}

- (void)touchesEnded:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] deckView:self endedTouches:touches];
}

@end
