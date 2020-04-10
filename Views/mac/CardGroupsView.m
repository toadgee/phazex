//
//  CardGroupsView.m
//  phazex
//
//  Created by toddha on 12/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardGroupsView.h"
#import "CardStackView.h"
#import "CardGroupsArtist.h"
#import "PfmView.h"

@interface CardGroupsView () <CardStackViewDelegate, PXCardGroupsArtistDelegate>
{
	PXCardGroupsArtist *_artist;
}
@end

@implementation CardGroupsView

@synthesize delegate = _delegate;

- (id)initWithFrame:(NSRect)frameRect
{
	self = [super initWithFrame:frameRect];
	
	if (self)
	{
		_artist = [[PXCardGroupsArtist alloc] init];
		[_artist setDelegate:self];
		[self setWantsLayer:YES];
	}
	
	return self;
}

- (void)dealloc
{
	[_artist release];
	[super dealloc];
}

- (BOOL)mouseDownCanMoveWindow
{
	return NO;
}

- (NSArray<PXGroup *> *)groups
{
	return [_artist groups];
}

- (void)setGroups:(NSArray<PXGroup *> *)groups
{
	[_artist setGroups:groups];
}

- (PXGroup *)currentGroup
{
	return [_artist currentGroup];
}

- (IBAction)nextGroup:(id)sender
{
	[_artist nextGroup];
}

- (IBAction)previousGroup:(id)sender
{
	[_artist previousGroup];
}

- (void) mouseDown:(NSEvent*)event
{	
	NSPoint point = [self convertPoint:[event locationInWindow] fromView:nil];
	[self clickedAtPoint:point clickCount:(int)event.clickCount onCardGroup:nil onCardView:nil isMainCardGroup:NO defaultTransition:YES];
}

-(void)cardStackView:(CardStackView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardView:(CardView *)cardView
{
	CardStackView *frontGroup = nil;
	
	int ptr = [_artist ptr];
	if (ptr == -1)
	{
		NSLog(@"ERROR - ptr is -1");
		return;
	}
	
	if ([[self subviews] count] > ptr)
	{
		frontGroup = [[self subviews] objectAtIndex:ptr];
	}
	
	[self clickedAtPoint:[self convertPoint:point fromView:nil] clickCount:clicks onCardGroup:view onCardView:cardView isMainCardGroup:(view == frontGroup) defaultTransition:(cardView == nil)];
}

-(void)clickedAtPoint:(CGPoint)point clickCount:(int)clicks onCardGroup:(CardStackView *)cardGroup onCardView:(CardView *)cardView isMainCardGroup:(BOOL)isMain defaultTransition:(BOOL)transitionGroups
{
	if (cardView == nil)
	{
		// TODO : Why does this suck and not give us the card we actually want?!?
		cardView = [cardGroup cardViewAtPoint:[self convertPoint:point fromView:nil]];
	}
	
	if ([_delegate respondsToSelector:@selector(cardGroupsView:mouseClickedAtPoint:clickCount:onCardGroup:onCardView:isMainCardGroup:movesGroups:)])
	{
		[_delegate cardGroupsView:self mouseClickedAtPoint:point clickCount:clicks onCardGroup:cardGroup onCardView:cardView isMainCardGroup:isMain movesGroups:&transitionGroups];
	}
	
	if (transitionGroups)
	{
		CGFloat previousClickThreshhold = [self bounds].size.width / 2.0;
		CGFloat nextClickThreshhold = [self bounds].size.width / 2.0;
		if (point.x <= previousClickThreshhold)
			[self nextGroup:nil];
		else if (point.x >= nextClickThreshhold)
			[self previousGroup:nil];
	}
}

- (void)drawRect:(NSRect)dirtyRect
{
	[_artist drawRect:dirtyRect];
}

- (CGRect)boundsForArtist:(PXCardGroupsArtist *)artist
{
	return [self bounds];
}

- (NSArray<PfmView *> *)subviewsForArtist:(PXCardGroupsArtist *)artist
{
	return [self subviews];
}

- (PfmView *)cardGroupsArtist:(PXCardGroupsArtist *)artist stackViewForGroup:(PXGroup *)group
{
	NSRect stackViewFrame = NSMakeRect(0, 0, 0, 0);
	CardStackView *stackView = [[[CardStackView alloc] initWithFrame:stackViewFrame] autorelease];
	[stackView setDelegate:self];
	[stackView setHovers:NO];
	[stackView setCards:group];
	return stackView;
}

- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist updateSubviews:(NSArray<PfmView *> *)subviews
{
	[self setSubviews:subviews];
}

- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist groupViewNeedsDisplay:(PfmView *)groupView
{
	[groupView setNeedsDisplay:YES];
}


@end
