//
//  CardCollectionView.m
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import "CardCollectionView.h"
#import "CardCollection.h"
#import "CardStackArtist.h"
#import "CardView.h"
#import "Card.h"

@interface PXCardCollectionView () <PXCardStackArtistDelegate, PXCardViewDelegate>
{
@public
	PXCardStackArtist *_artist;
}
@end

void PXCardCollectionView_Init(PXCardCollectionView *view)
{
	view->_artist = [[PXCardStackArtist alloc] init];
	[view->_artist setDelegate:view];
	[view->_artist setFlipped:YES];
}

@implementation PXCardCollectionView

- (instancetype)initWithCoder:(NSCoder *)decoder
{
	self = [super initWithCoder:decoder];
	if (self)
	{
		PXCardCollectionView_Init(self);
	}
	
	return self;
}

- (void)dealloc
{
	[_artist setDelegate:nil];
	[_artist release];
	[super dealloc];
}

- (instancetype)initWithFrame:(CGRect)frame
{
	self = [super initWithFrame:frame];
	if (self)
	{
		PXCardCollectionView_Init(self);
	}
	
	return self;
}

- (void)setUserInteractionEnabled:(BOOL)userInteractionEnabled
{
	[_artist setEnabled:userInteractionEnabled];
	[super setUserInteractionEnabled:userInteractionEnabled];
}

- (void)setCardCollection:(PXCardCollection *)cardCollection
{
	[_artist setCards:cardCollection];
}

- (void)cardStackArtistUpdateView:(PXCardStackArtist *)artist
{
	[self setNeedsDisplay];
}

- (void)resizeSubviewsWithOldSize:(CGSize)oldBoundsSize
{	
	[_artist recalculatePositions:NO];
	[self setNeedsDisplay];
}

- (void)cardStackArtist:(PXCardStackArtist *)artist updateSubviewsForCollection:(PXCardCollection *)collection
{
	// TODO : we can simply update the subviews instead of recreating them all of the time!
	{
		NSArray<UIView *> *subviews = [self subviews];
		for (UIView *view in subviews)
		{
			if ([view isKindOfClass:[PXCardView class]])
			{
				[(PXCardView *)view setDelegate:nil];
			}
			
			[view removeFromSuperview];
		}
	}
	
	for (int i = 0; i < [collection count]; i++)
	{
		PXCard *card = [collection cardAtIndex:i];
		CGRect viewFrame = CGRectMake(0, 0, 0, 0); // just the default frame
		PXCardView * view = [[[PXCardView alloc] initWithFrame:viewFrame] autorelease];
		[view setDelegate:self];
		[view setCard:card];
		[self addSubview:view];
	}
	
	[_artist recalculatePositions:NO];
	[self setNeedsDisplay];
}

- (void)setHovers:(BOOL)hovers { [_artist setHovers:hovers]; }

- (void)cardStackArtistUpdateTrackingArea:(PXCardStackArtist *)artist
{
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
	return [(PXCardView *)view card];
}

- (void)cardView:(PXCardView *)cardView beganTouches:(NSSet<UITouch *> *)touches
{
	[self processTouchesForCardView:cardView unselect:YES];
}

- (void)cardView:(PXCardView *)cardView movedTouches:(NSSet<UITouch *> *)touches
{
//	[self processTouchesForCardView:cardView unselect:YES];
}

- (void)cardView:(PXCardView *)cardView endedTouches:(NSSet<UITouch *> *)touches
{
//	[self processTouchesForCardView:cardView unselect:YES];
}

- (void)processTouchesForCardView:(PXCardView *)cardView unselect:(BOOL)unselect
{
	PXCard *card = [cardView card];
	int cardId = kCardInvalidId;
	if (card)
	{
		cardId = CardId(card);
	}
	
	if ([_artist hoveringCardId] == cardId && unselect)
	{
		cardId = kCardInvalidId;
	}
	
	[_artist setHoveringCardId:cardId];
}

- (PXCard *)selectedCard
{
	PXCard *card = NULL;
	int cardId = [_artist hoveringCardId];
	if (cardId != kCardInvalidId)
	{
		card = [[_artist cards] cardWithId:cardId];
	}
	
	return card;
}

@end
