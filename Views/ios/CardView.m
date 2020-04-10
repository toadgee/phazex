//
//  CardView.m
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import <UIKit/UIKit.h>
#import "CardView.h"
#import "CardArtist.h"

@interface PXCardView () <PXCardArtistDelegate>
{
@public
	PXCardArtist *_artist;
}

@end

void PXCardViewCommonInit(PXCardView *cardView);
void PXCardViewCommonInit(PXCardView *cardView)
{
	cardView->_artist = [[PXCardArtist alloc] init];
	[cardView->_artist setCard:CreateCard(GetUICardPool(), 5, 3, 0)];
	[cardView->_artist setDelegate:cardView];
	[[cardView layer] setAffineTransform:CGAffineTransformMakeScale(1, 1)];
}

@implementation PXCardView

- (instancetype)initWithCoder:(NSCoder *)decoder
{
	self = [super initWithCoder:decoder];
	if (self)
	{
		PXCardViewCommonInit(self);
	}
	
	return self;
}
- (instancetype)initWithFrame:(CGRect)frame
{
	self = [super initWithFrame:frame];
	if (self)
	{
		PXCardViewCommonInit(self);
	}
	
	return self;
}

- (void)dealloc
{
	[_artist release];
	[super dealloc];
}

- (void)updateViewForArtist:(PXCardArtist *)artist
{
	[self setNeedsDisplay];
}

- (void)setCard:(PXCard *)card
{
	[_artist setCard:card];
}

- (PXCard *)card
{
	return [_artist card];
}

- (void)drawRect:(CGRect)rect
{
    [_artist drawInRect:rect];
}

- (void)touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] cardView:self beganTouches:touches];
}

- (void)touchesMoved:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] cardView:self movedTouches:touches];
}

- (void)touchesEnded:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event
{
	[[self delegate] cardView:self endedTouches:touches];
}

@end
