//
//  CardStackArtist.m
//  phazex
//
//  Created by Todd Harris on 11/19/16.
//
//

#import "CardStackArtist.h"
#import "CardCollection.h"
#import "PfmView.h"
#import "CardArtist.h"

@interface PXCardStackArtist () {
	PXCardCollection *_cards;
	
	int _hoveringCardId;
	
	BOOL _flipped;
	BOOL _fanEffect;
	BOOL _stacked;
	BOOL _hovers;
	BOOL _enabled;
	
	BOOL _verticallyCentered;
	BOOL _horizontallyCentered;
}

@end

@implementation PXCardStackArtist

- (instancetype)init
{
	self = [super init];
	if (self)
	{
		_hovers = YES;
		_stacked = NO;
		_enabled = YES;
		_fanEffect = YES;
		_horizontallyCentered = YES;
		_verticallyCentered = YES;
		_cards = [[PXCardCollection alloc] init];
	}
	
	return self;
}

- (void)dealloc
{
	[_cards release];
	[super dealloc];
}

- (PXCardCollection *)cards
{
	return _cards;
}

- (void)setCards:(PXCardCollection *)collection
{
	if (_cards != nil)
	{
		[_cards release];
	}
	
	_cards = [collection copyWithPool:GetUICardPool()];
	[_cards sortByNumber];
	
	[[self delegate] cardStackArtist:self updateSubviewsForCollection:_cards];
}

-(void)setHoveringCardId:(int)hoveringCardId
{
	if (hoveringCardId != _hoveringCardId)
	{
		_hoveringCardId = hoveringCardId;
		[self recalculatePositions:NO];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (BOOL)isFlipped { return _flipped; }
- (BOOL)doesHover { return _hovers; }
- (BOOL)isVerticallyCentered { return _verticallyCentered; }
- (BOOL)horizontallyCentered { return _horizontallyCentered; }
- (BOOL)hasFanEffect { return _fanEffect; }
- (BOOL)isStacked { return _stacked; }
- (BOOL)isEnabled { return _enabled; }

- (void)setFlipped:(BOOL)flipped
{
	if (_flipped != flipped)
	{
		_flipped = flipped;
		[self recalculatePositions:YES];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setHovers:(BOOL)hovers
{
	if (_hovers != hovers)
	{
		_hovers = hovers;
		[self recalculatePositions:YES];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setFanEffect:(BOOL)fanEffect
{
	if (_fanEffect != fanEffect)
	{
		_fanEffect = fanEffect;
		[self recalculatePositions:NO];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setVerticallyCentered:(BOOL)centered
{
	if (_verticallyCentered != centered)
	{
		_verticallyCentered = centered;
		[self recalculatePositions:NO];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setHorizontallyCentered:(BOOL)centered
{
	if (_horizontallyCentered != centered)
	{
		_horizontallyCentered = centered;
		[self recalculatePositions:NO];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setStacked:(BOOL)stacked
{
	if (_stacked != stacked)
	{
		_stacked = stacked;
		[self recalculatePositions:NO];
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

- (void)setEnabled:(BOOL)enabled
{
	if (enabled != _enabled)
	{
		_enabled = enabled;
		[[self delegate] cardStackArtistUpdateView:self];
	}
}

-(void)recalculatePositions:(BOOL)useAnimation
{
	[[self delegate] cardStackArtistUpdateTrackingArea:self];
	
	NSArray<PfmView *> *subviews = [[self delegate] cardStackArtistSubviews:self];
	int subviewCount = (int)[subviews count];
	
	const CGFloat hoveringAmount = 10;
	CGRect bounds = [[self delegate] cardStackArtistBounds:self];
	CGFloat frameWidth = bounds.size.width;
	CGFloat frameHeight = bounds.size.height;
	if (frameWidth == 0 || frameHeight == 0) {
		return;
	}
	
	if (_hovers)
	{
		if (_flipped)
		{
			frameHeight += hoveringAmount;
		}
		else
		{
			frameHeight -= hoveringAmount;
		}
	}
	
	CGFloat cardWidth;
	CGFloat cardSpacing;
	if (_stacked)
	{
		cardWidth = frameWidth;
		cardSpacing = 0;
	}
	else
	{
		cardSpacing = frameWidth / (subviewCount + 1);
		cardWidth = cardSpacing * 2;
	}
	
	CGFloat cardHeight = [PXCardArtist preferredCardHeightForWidth:cardWidth];
	if (cardHeight > frameHeight)
	{
		cardWidth = [PXCardArtist preferredCardWidthForHeight:frameHeight] * 2;
		cardHeight = frameHeight;
		cardSpacing = cardWidth / 2;
		
		if (_stacked)
		{
			cardSpacing = 0;
		}
	}
	
	CGFloat totalWidth;
	if (_stacked)
	{
		totalWidth = cardWidth;
	}
	else
	{
		totalWidth = cardSpacing * (subviewCount + 1);
	}
	
	CGFloat xPos = 0;
	if (_horizontallyCentered)
	{
		xPos = (totalWidth < frameWidth)  ? (frameWidth / 2) - (totalWidth / 2) : 0;
	}
	
	CGFloat yPos = 0;
	if (_verticallyCentered)
	{
		yPos = (cardHeight < frameHeight) ? (frameHeight / 2) - (cardHeight / 2) : 0;
	}
	
	CGFloat rotation = 0.0;
	CGFloat rotationDelta = 0.0;
	if (_fanEffect)
	{
		if (subviewCount > 1)
		{
			rotationDelta = -1.5;
			
			// determine the initial rotation
			rotation = (subviewCount / -2) * rotationDelta;
		}
	}
	
	int cardIndex = 0;
	for (PfmView *card in subviews)
	{
		id layer = card;
		if (useAnimation)
			layer = PfmViewGetAnimator(card);
		
		CGRect rect;
		if (!_hovers || _hoveringCardId != CardId([[self delegate] cardStackArtist:self cardForView:card]))
		{
			rect = CGRectMake(xPos + cardSpacing * cardIndex, yPos, cardWidth, cardHeight);
		}
		else
		{
			CGFloat hoveredYPos = yPos;
			if ([self isFlipped])
			{
				hoveredYPos -= hoveringAmount;
			}
			else
			{
				hoveredYPos += hoveringAmount;
			}
			rect = CGRectMake(xPos + cardSpacing* cardIndex, hoveredYPos, cardWidth, cardHeight);
		}
		
		/*if (_fanEffect)
		 {
			[layer setBoundsRotation:rotation];
			[layer setFrameCenterRotation:rotation];
		 }*/
		
		[layer setFrame:rect];
		
		cardIndex++;
		rotation += rotationDelta;
	}
}

//- (CGRect)actualFrameForFrame:(CGRect)frame
//{
//	NSArray *subviews = [self subviews];
//	int subviewCount = (int)[subviews count];
//	
//	if (subviewCount == 0)
//	{
//		return CGRectMake(frame.origin.x + frame.size.width / 2, frame.origin.y + frame.size.height / 2, 0, 0);
//	}
//	
//	CGFloat frameWidth = frame.size.width;
//	CGFloat frameHeight = frame.size.height;
//	
//	CGFloat cardWidth;
//	CGFloat cardSpacing;
//	if (_stacked)
//	{
//		cardWidth = frameWidth;
//		cardSpacing = 0;
//	}
//	else
//	{
//		cardSpacing = frameWidth / (subviewCount + 1);
//		cardWidth = cardSpacing * 2;
//	}
//	
//	CGFloat cardHeight = [PXCardArtist preferredCardHeightForWidth:cardWidth];
//	if (cardHeight > frameHeight)
//	{
//		cardWidth = [PXCardArtist preferredCardWidthForHeight:frameHeight];
//		cardHeight = frameHeight;
//		cardSpacing = cardWidth / 2;
//		
//		if (_stacked)
//		{
//			cardSpacing = 0;
//		}
//	}
//	
//	CGFloat totalWidth;
//	if (_stacked)
//	{
//		totalWidth = cardWidth;
//	}
//	else
//	{
//		totalWidth = cardSpacing * (subviewCount + 1);
//	}
//	
//	CGFloat actualX = frame.origin.x + (frame.size.width - totalWidth) / 2;
//	CGFloat actualY = frame.origin.y - (frame.size.height - cardHeight) / 2;
//	
//	return CGRectMake(actualX, actualY, cardHeight, totalWidth);
//}

@end
