//
//  CardGroupsView.m
//  phazex
//
//  Created by toddha on 12/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardGroupsView.h"
#import "CardCollectionView.h"
#import "CardGroupsArtist.h"
#import "PfmView.h"

@interface PXCardGroupsView () <PXCardGroupsArtistDelegate>
{
@public
	PXCardGroupsArtist *_artist;
}
@end

void PXCardGroupsViewCommonInit(PXCardGroupsView *cardGroupsView);
void PXCardGroupsViewCommonInit(PXCardGroupsView *cardGroupsView)
{
	cardGroupsView->_artist = [[PXCardGroupsArtist alloc] init];
	[cardGroupsView->_artist setDelegate:cardGroupsView];
}


@implementation PXCardGroupsView

@synthesize delegate = _delegate;

- (instancetype)initWithCoder:(NSCoder *)decoder
{
	self = [super initWithCoder:decoder];
	if (self)
	{
		PXCardGroupsViewCommonInit(self);
	}
	
	return self;
}
- (instancetype)initWithFrame:(CGRect)frame
{
	self = [super initWithFrame:frame];
	if (self)
	{
		PXCardGroupsViewCommonInit(self);
	}
	
	return self;
}

- (void)dealloc
{
	[_artist release];
	[super dealloc];
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

//- (void) mouseDown:(NSEvent*)event
//{	
//	NSPoint point = [self convertPoint:[event locationInWindow] fromView:nil];
//	[self clickedAtPoint:point clickCount:(int)event.clickCount onCardGroup:nil onCardView:nil isMainCardGroup:NO defaultTransition:YES];
//}
//
//-(void)cardStackView:(CardStackView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardView:(CardView *)cardView
//{
//	CardStackView *frontGroup = nil;
//	
//	int ptr = [_artist ptr];
//	if (ptr == -1)
//	{
//		NSLog(@"ERROR - ptr is -1");
//		return;
//	}
//	
//	if ([[self subviews] count] > ptr)
//	{
//		frontGroup = [[self subviews] objectAtIndex:ptr];
//	}
//	
//	[self clickedAtPoint:[self convertPoint:point fromView:nil] clickCount:clicks onCardGroup:view onCardView:cardView isMainCardGroup:(view == frontGroup) defaultTransition:(cardView == nil)];
//}
//
//-(void)clickedAtPoint:(CGPoint)point clickCount:(int)clicks onCardGroup:(CardStackView *)cardGroup onCardView:(CardView *)cardView isMainCardGroup:(BOOL)isMain defaultTransition:(BOOL)transitionGroups
//{
//	if (cardView == nil)
//	{
//		// TODO : Why does this suck and not give us the card we actually want?!?
//		cardView = [cardGroup cardViewAtPoint:[self convertPoint:point fromView:nil]];
//	}
//	
//	if ([_delegate respondsToSelector:@selector(cardGroupsView:mouseClickedAtPoint:clickCount:onCardGroup:onCardView:isMainCardGroup:movesGroups:)])
//	{
//		[_delegate cardGroupsView:self mouseClickedAtPoint:point clickCount:clicks onCardGroup:cardGroup onCardView:cardView isMainCardGroup:isMain movesGroups:&transitionGroups];
//	}
//	
//	if (transitionGroups)
//	{
//		CGFloat previousClickThreshhold = [self bounds].size.width / 2.0;
//		CGFloat nextClickThreshhold = [self bounds].size.width / 2.0;
//		if (point.x <= previousClickThreshhold)
//			[self nextGroup:nil];
//		else if (point.x >= nextClickThreshhold)
//			[self previousGroup:nil];
//	}
//}

- (void)drawRect:(CGRect)dirtyRect
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
	CGRect stackViewFrame = CGRectMake(0, 0, 0, 0);
	PXCardCollectionView *stackView = [[[PXCardCollectionView alloc] initWithFrame:stackViewFrame] autorelease];
//	[stackView setDelegate:self];
	[stackView setHovers:NO];
	[stackView setCardCollection:group];
	return stackView;
}

- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist updateSubviews:(NSArray<PfmView *> *)newSubviews
{
	NSArray<UIView *> *subviews = [self subviews];
	for (UIView *view in subviews)
	{
		if ([view isKindOfClass:[PXCardCollectionView class]])
		{
			//[(PXCardCollectionView *)view setDelegate:nil];
		}
		
		[view removeFromSuperview];
	}
	
	for (UIView *view in newSubviews)
	{
		[self addSubview:view];
	}
}

- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist groupViewNeedsDisplay:(PfmView *)groupView
{
	[groupView setNeedsDisplay];
}


@end
