//
//  CardStackView.m
//  phazex
//
//  Created by toddha on 11/14/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardStackView.h"
#import "CardView.h"
#import "CardArtist.h"
#import "CardStackArtist.h"

@interface CardStackView () <PXCardStackArtistDelegate> {
	CardView *_selectedCardView;
}
@end

@implementation CardStackView

@synthesize delegate = _delegate;

- (id)initWithFrame:(NSRect)frame
{
    self = [super initWithFrame:frame];

    if (self)
	{
		_artist = [[PXCardStackArtist alloc] init];
		[_artist setDelegate:self];
		[self setWantsLayer:YES];
    }

    return self;
}

-(void)dealloc
{
	[self removeTrackingArea:_trackingArea];
	[_artist release];
	[_trackingArea release];
	[super dealloc];
}

- (CardView *)cardViewAtPoint:(NSPoint)point
{
	NSArray<NSView *> *subviews = [self subviews];
	for (int i = ((int)[subviews count] - 1); i >= 0; i--)
	{
		CardView *card = (CardView *)[subviews objectAtIndex:i];
		CGRect frame = [card frame];
		if (CGRectContainsPoint(frame, point))
		{
			return card;
		}
	}
	
	return nil;
}

- (CardView *)selectedCardView
{
	return [[_selectedCardView retain] autorelease];
}

- (void)setSelectedCardView:(CardView *)cardView
{
	if (_selectedCardView != cardView)
	{
		[_selectedCardView setSelected:NO];
		[_selectedCardView release];
		_selectedCardView = [cardView retain];
		[_selectedCardView setSelected:YES];
	}
	else
	{
		[_selectedCardView setSelected:![_selectedCardView isSelected]];
	}
	
	[_selectedCardView setNeedsDisplay:YES];
}

- (void) mouseEntered:(NSEvent*)event
{
	
}

- (void) mouseDown:(NSEvent*)event
{
	if (![_artist isEnabled]) return;
	
	if ([_delegate respondsToSelector:@selector(cardStackView:mouseClickedAtPoint:clickCount:onCardView:)])
	{
		NSPoint point = [self convertPoint:[event locationInWindow] fromView:nil];
		CardView *cardView = [self cardViewAtPoint:point];
		[self setSelectedCardView:cardView];
		[_delegate cardStackView:self mouseClickedAtPoint:point clickCount:(int)event.clickCount onCardView:cardView];
	}
}

- (void) mouseMoved:(NSEvent*)event
{
	NSPoint point = [self convertPoint:[event locationInWindow] fromView:nil];
	CardView *cardView = [self cardViewAtPoint:point];
	PXCard *card = [cardView card];
	int cardId = kCardInvalidId;
	if (card)
	{
		cardId = CardId(card);
	}
	
	[_artist setHoveringCardId:cardId];
}

- (void) mouseExited:(NSEvent*)event
{
	[_artist setHoveringCardId:kCardInvalidId];
}

- (void)updateTrackingArea
{
	[self removeTrackingArea:_trackingArea];
	[_trackingArea release];
	
	NSTrackingAreaOptions options = (NSTrackingMouseEnteredAndExited | NSTrackingMouseMoved | NSTrackingActiveAlways);
	_trackingArea = [[NSTrackingArea alloc] initWithRect:[self bounds] options:options owner:self userInfo:nil];
	[self addTrackingArea:_trackingArea];
}

- (void)resizeSubviewsWithOldSize:(NSSize)oldBoundsSize
{	
	[self updateTrackingArea];
	[_artist recalculatePositions:NO];
	[self setNeedsDisplay:YES];
}

- (BOOL)mouseDownCanMoveWindow
{
	return NO;
}

- (BOOL)acceptsFirstResponder
{
	return YES;
}

- (BOOL)resignFirstResponder
{
	[self setNeedsDisplay:YES];
	return YES;
}

- (BOOL)becomeFirstResponder
{
	if (![_artist isEnabled]) return NO;
	[self setNeedsDisplay:YES];
	return YES;	
}

-(void)drawRect:(NSRect)rect
{
	[[NSColor redColor] setFill];
	[NSBezierPath fillRect:rect];
}

- (void)setEnabled:(BOOL)enabled { [_artist setEnabled:enabled]; }
- (void)setFanEffect:(BOOL)fanEffect { [_artist setFanEffect:fanEffect]; }
- (void)setStacked:(BOOL)stacked { [_artist setStacked:stacked]; }
- (void)setHovers:(BOOL)hovers { [_artist setHovers:hovers]; }
- (void)setCards:(PXCardCollection *)cards { [_artist setCards:cards]; }

#pragma mark -
#pragma mark PXCardStackArtistDelegate
- (void)cardStackArtistUpdateView:(PXCardStackArtist *)artist
{
	[self setNeedsDisplay:YES];
}

- (void)cardStackArtist:(PXCardStackArtist *)artist updateSubviewsForCollection:(PXCardCollection *)collection
{
	NSMutableArray *newSubviews = [NSMutableArray arrayWithCapacity:[collection count]];
	// TODO : we can simply update the subviews instead of recreating them all of the time!
	for (int i = 0; i < [collection count]; i++)
	{
		PXCard *card = [collection cardAtIndex:i];
		NSRect viewFrame = NSMakeRect(0, 0, 0, 0); // just the default frame
		CardView * view = [[[CardView alloc] initWithFrame:viewFrame] autorelease];
		[view setCard:card];
		
		[newSubviews addObject:view];
	}
	
	[self setSubviews:newSubviews];
	[_artist recalculatePositions:NO];
	[self setNeedsDisplay:YES];
}

- (void)cardStackArtistUpdateTrackingArea:(PXCardStackArtist *)artist
{
	[self updateTrackingArea];
}

- (CGRect)cardStackArtistBounds:(PXCardStackArtist *)artist
{
	return [self bounds];
}

- (NSArray<PfmView *> *)cardStackArtistSubviews:(PXCardStackArtist *)artist
{
	return [self subviews];
}

- (PXCard *)cardStackArtist:(PXCardStackArtist *)artist cardForView:(PfmView *)view
{
	return [(CardView *)view card];
}

@end
