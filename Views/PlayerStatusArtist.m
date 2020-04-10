//
//  PlayerStatusArtist.m
//  phazex
//
//  Created by toddha on 11/26/16.
//
//

#import "PfmColor.h"
#import "PfmBezierPath.h"
#import "Player.h"
#import "PlayerStatusArtist.h"
#import "PfmFont.h"

@interface PXPlayerStatusArtist () {
	// colors for when it's not the player's turn and they haven't made their phaze
	PfmColor            *_backgroundInactiveNotMadePhazeColor;
	PfmColor            *_foregroundInactiveNotMadePhazeColor;
	PfmColor            *_borderInactiveNotMadePhazeColor;
	
	// colors for when it's the player's turn and they haven't made their phaze
	PfmColor            *_backgroundActiveNotMadePhazeColor;
	PfmColor            *_foregroundActiveNotMadePhazeColor;
	PfmColor            *_borderActiveNotMadePhazeColor;
	
	// colors for when it's not the player's turn and they have made their phaze
	PfmColor            *_backgroundInactiveMadePhazeColor;
	PfmColor            *_foregroundInactiveMadePhazeColor;
	PfmColor            *_borderInactiveMadePhazeColor;

	// colors for when it's the player's turn and they have made their phaze
	PfmColor            *_backgroundActiveMadePhazeColor;
	PfmColor            *_foregroundActiveMadePhazeColor;
	PfmColor            *_borderActiveMadePhazeColor;
	
	// colors for when the player is skipped
	PfmColor            *_backgroundSkippedColor;
	PfmColor            *_foregroundSkippedColor;
	PfmColor            *_borderSkippedColor;
	
	// the players
	NSArray<PXPlayer *>   *_players;
}

// note that these should be manually implemented, but for now just remember to say setNeedsDisplay:YES
@property (readwrite, retain) PfmColor *backgroundInactiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *foregroundInactiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *borderInactiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *backgroundActiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *foregroundActiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *borderActiveNotMadePhazeColor;
@property (readwrite, retain) PfmColor *backgroundInactiveMadePhazeColor;
@property (readwrite, retain) PfmColor *foregroundInactiveMadePhazeColor;
@property (readwrite, retain) PfmColor *borderInactiveMadePhazeColor;
@property (readwrite, retain) PfmColor *backgroundActiveMadePhazeColor;
@property (readwrite, retain) PfmColor *foregroundActiveMadePhazeColor;
@property (readwrite, retain) PfmColor *borderActiveMadePhazeColor;
@property (readwrite, retain) PfmColor *backgroundSkippedColor;
@property (readwrite, retain) PfmColor *foregroundSkippedColor;
@property (readwrite, retain) PfmColor *borderSkippedColor;
@end

@implementation PXPlayerStatusArtist
@synthesize backgroundInactiveNotMadePhazeColor = _backgroundInactiveNotMadePhazeColor;
@synthesize foregroundInactiveNotMadePhazeColor = _foregroundInactiveNotMadePhazeColor;
@synthesize borderInactiveNotMadePhazeColor = _borderInactiveNotMadePhazeColor;
@synthesize backgroundActiveNotMadePhazeColor = _backgroundActiveNotMadePhazeColor;
@synthesize foregroundActiveNotMadePhazeColor = _foregroundActiveNotMadePhazeColor;
@synthesize borderActiveNotMadePhazeColor = _borderActiveNotMadePhazeColor;
@synthesize backgroundInactiveMadePhazeColor = _backgroundInactiveMadePhazeColor;
@synthesize foregroundInactiveMadePhazeColor = _foregroundInactiveMadePhazeColor;
@synthesize borderInactiveMadePhazeColor = _borderInactiveMadePhazeColor;
@synthesize backgroundActiveMadePhazeColor = _backgroundActiveMadePhazeColor;
@synthesize foregroundActiveMadePhazeColor = _foregroundActiveMadePhazeColor;
@synthesize borderActiveMadePhazeColor = _borderActiveMadePhazeColor;
@synthesize backgroundSkippedColor = _backgroundSkippedColor;
@synthesize foregroundSkippedColor = _foregroundSkippedColor;
@synthesize borderSkippedColor = _borderSkippedColor;
@synthesize delegate = _delegate;

- (instancetype)init
{
	self = [super init];
	if (self)
	{
		[self setBackgroundInactiveNotMadePhazeColor:PfmColorWithRGB(0.0, 0.5, 0.0, 1.0)];
		[self setForegroundInactiveNotMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		[self setBorderInactiveNotMadePhazeColor:PfmColorWithRGB(0.0, 0.8, 0.0, 1.0)];
		
		[self setBackgroundActiveNotMadePhazeColor:PfmColorWithRGB(0.0, 0.8, 0.0, 1.0)];
		[self setForegroundActiveNotMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		[self setBorderActiveNotMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		
		[self setBackgroundInactiveMadePhazeColor:PfmColorWithRGB(0.5, 0.5, 0.0, 1.0)];
		[self setForegroundInactiveMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		[self setBorderInactiveMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 0.0, 1.0)];
		
		[self setBackgroundActiveMadePhazeColor:PfmColorWithRGB(0.8, 0.8, 0.0, 1.0)];
		[self setForegroundActiveMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		[self setBorderActiveMadePhazeColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		
		[self setBackgroundSkippedColor:PfmColorWithRGB(0.5, 0.0, 0.0, 1.0)];
		[self setForegroundSkippedColor:PfmColorWithRGB(1.0, 1.0, 1.0, 1.0)];
		[self setBorderSkippedColor:PfmColorWithRGB(0.8, 0.0, 0.0, 1.0)];
	}
	
	return self;
}

-(void)dealloc
{
	[_backgroundInactiveNotMadePhazeColor release];
	[_foregroundInactiveNotMadePhazeColor release];
	[_borderInactiveNotMadePhazeColor release];
	[_backgroundActiveNotMadePhazeColor release];
	[_foregroundActiveNotMadePhazeColor release];
	[_borderActiveNotMadePhazeColor release];
	[_backgroundInactiveMadePhazeColor release];
	[_foregroundInactiveMadePhazeColor release];
	[_borderInactiveMadePhazeColor release];
	[_backgroundActiveMadePhazeColor release];
	[_foregroundActiveMadePhazeColor release];
	[_borderActiveMadePhazeColor release];
	[_backgroundSkippedColor release];
	[_foregroundSkippedColor release];
	[_borderSkippedColor release];
	[_players release];
	[super dealloc];
}

-(NSArray<PXPlayer *> *)players
{
	return _players;
}

-(void)setPlayers:(NSArray<PXPlayer *> *)players
{
	_players = [[NSArray arrayWithArray:players] retain];
	[[self delegate] playerStatusArtistsNeedsDisplay:self];
}

- (void)drawString:(NSString*)string centeredIn:(CGRect)rect withAttributes:(NSDictionary*)attr
{
	CGSize stringSize = [string sizeWithAttributes:attr];
	
	CGRect centeredRect = CGRectMake(
		rect.origin.x + (rect.size.width / 2) - (stringSize.width / 2),
		rect.origin.y + (rect.size.height / 2) - (stringSize.height / 2),
		stringSize.width,
		stringSize.height);
	
	[string drawInRect:centeredRect withAttributes:attr];
}

- (void)drawRect:(CGRect)rect
{
	/*if (_backgroundColor)
	{
		[_backgroundColor setFill];
		CGFillRect(rect);
	}*/
	
	if (_players == nil) return;
	if ([_players count] == 0) return;
	
	// spacing between each item as well as at the beginning & end
	CGFloat spacing = 5;
	
	// figure out how big each player's rectangle should be
	CGFloat width = rect.size.width - 2 * spacing;
	CGFloat height = (rect.size.height - (([_players count] + 1) * spacing)) / [_players count];
	
	// the position where the player's rectangle is -- for the first one, we want spacing + spacing
	CGFloat x = spacing;
	CGFloat y = spacing;
	
	for (PXPlayer *player in _players)
	{
		// figure out the colors
		PfmColor *backColor;
		PfmColor *textColor;
		//PfmColor *borderColor;
		if ([player skipCount] > 0)
		{
			backColor = _backgroundSkippedColor;
			textColor = _foregroundSkippedColor;
			//borderColor = _borderSkippedColor;
		}
		else if ([player completedPhaze])
		{
			if ([player isMyTurn])
			{
				backColor = _backgroundActiveMadePhazeColor;
				textColor = _foregroundActiveMadePhazeColor;
				//borderColor = _borderActiveMadePhazeColor;
			}
			else
			{
				backColor = _backgroundInactiveMadePhazeColor;
				textColor = _foregroundInactiveMadePhazeColor;
				//borderColor = _borderInactiveMadePhazeColor;
			}
		}
		else
		{
			if ([player isMyTurn])
			{
				backColor = _backgroundActiveNotMadePhazeColor;
				textColor = _foregroundActiveNotMadePhazeColor;
				//borderColor = _borderActiveNotMadePhazeColor;
			}
			else
			{
				backColor = _backgroundInactiveNotMadePhazeColor;
				textColor = _foregroundInactiveNotMadePhazeColor;
				//borderColor = _borderInactiveNotMadePhazeColor;
			}
		}
		
		// calculate the player's rectangle
		CGRect playerRect = CGRectMake(x, y, width, height);
		
		if (backColor)
		{
			[backColor setFill];
			PfmBezierPathFillRect(playerRect);
		}

		// TODO		
		/*if (_borderColor)
		{
			[borderColor set];
			[backPath setLineWidth:1.0f];
			[backPath stroke];
		}*/
	
		if (textColor && [player name])
		{
			NSMutableDictionary *smallTextAttr = [NSMutableDictionary dictionaryWithCapacity:5];
			[smallTextAttr setValue:[PfmFont fontWithName:@"Helvetica" size:(width / 12)] forKey:NSFontAttributeName];
			[smallTextAttr setValue:textColor forKey:NSForegroundColorAttributeName];
			[self drawString:[player name] centeredIn:playerRect withAttributes:smallTextAttr];
		}
		
		// increment the y, so we don't draw on top of ourselves
		y = y + height + spacing;
	}
}


@end
