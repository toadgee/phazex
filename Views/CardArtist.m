//
//  CardArtist.m
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import "PfmBezierPath.h"
#import "PfmFont.h"
#import "CardArtist.h"

@interface PXCardArtist () {
	PfmColor *_backgroundColor;
	PfmColor *_foregroundColor;
	PfmColor *_borderColor;
	PfmColor *_selectedColor;
	PfmColor *_selectedBorderColor;
	PfmColor *_unavailableColor;
	
	BOOL _unavailable;
	BOOL _selected;
	
	PXCard *_card;
}

@end

@implementation PXCardArtist

@synthesize foregroundColor = _foregroundColor;
@synthesize backgroundColor = _backgroundColor;
@synthesize borderColor = _borderColor;
@synthesize selectedColor = _selectedColor;
@synthesize selectedBorderColor = _selectedBorderColor;
@synthesize unavailableColor = _unavailableColor;

+(CGFloat)preferredCardWidthForHeight:(CGFloat)height
{
	return height * 0.35;
}

+(CGFloat)preferredCardHeightForWidth:(CGFloat)width
{
	return width / 0.7f;
}

- (instancetype)init {
	self = [super init];
	if (self) {
		_backgroundColor = [[PfmColor whiteColor] retain];
		_borderColor = [[PfmColor blackColor] retain];
		_selectedBorderColor = [PfmColorWithRGB(1.0, 1.0, 0.0, 1.0) retain];
		_selectedColor = [PfmColorWithRGB(1.0, 0.3, 0.0, 0.5) retain];
		_unavailableColor = [[[PfmColor darkGrayColor] colorWithAlphaComponent:0.4] retain];
	}
	
	return self;
}

- (void)dealloc
{
	[_unavailableColor release];
	[_backgroundColor release];
	[_foregroundColor release];
	[_borderColor release];
	[_selectedColor release];
	[_selectedBorderColor release];
	
	ReleaseCard(_card);
	
	[super dealloc];
}

-(NSString*) shortNumberString
{
	NSString *numberString = @"";
	if (_card)
	{
		if (CardNumber(_card) >= 1 && CardNumber(_card) <= 12)
		{
			numberString = [NSString stringWithFormat:@"%d", CardNumber(_card)];
		}
		else if (CardNumber(_card) == 0)
		{
			numberString = @"W";
		}
		else if (CardNumber(_card) == 13)
		{
			numberString = @"S";
		}
		else
		{
			numberString = @"?";
		}
	}
	
	return numberString;
}

-(NSString*) numberString
{
	NSString *numberString = @"";
	if (_card)
	{
		if (CardNumber(_card) >= 1 && CardNumber(_card) <= 12)
		{
			numberString = [NSString stringWithFormat:@"%d", CardNumber(_card)];
		}
		else if (CardNumber(_card) == 0)
		{
			numberString = @"Wild";
		}
		else if (CardNumber(_card) == 13)
		{
			numberString = @"Skip";
		}
		else
		{
			numberString = @"?";
		}
	}
	
	return numberString;
}

-(BOOL)isUnavailable
{
	return _unavailable;
}

-(void)setIsUnavailable:(BOOL)unavailable
{
	if (_unavailable != unavailable)
	{
		_unavailable = unavailable;
		[[self delegate] updateViewForArtist:self];
	}
}

-(BOOL)isSelected
{
	return _selected;
}

-(void)setIsSelected:(BOOL)isSelected
{
	if (isSelected != _selected)
	{
		_selected = isSelected;
		[[self delegate] updateViewForArtist:self];
	}
}

-(PXCard *)card
{
	return _card;
}

-(void)setCard:(PXCard *)card
{
	if (card != _card)
	{
		if (_card != NULL)
		{
			ReleaseCard(_card);
			_card = NULL;
		}
		
		self.foregroundColor = [PfmColor darkGrayColor];
		
		if (card)
		{
			_card = CopyCard(GetUICardPool(), card);
			switch (CardColor(_card))
			{
				case 0:
					self.foregroundColor = [PfmColor redColor];
					break;
					
				case 1:
					self.foregroundColor = PfmColorWithRGB(1.0f, 0.6f, 0.0f, 1.0f);
					break;
					
				case 2:
					self.foregroundColor = PfmColorWithRGB(0.0f, 0.7f, 0.0f, 1.0f);
					break;
					
				case 3:
					self.foregroundColor = [PfmColor blueColor];
					break;
			}
		}
		
		[[self delegate] updateViewForArtist:self];
	}
}

- (void)drawInRect:(CGRect)rect
{
	CGFloat x = rect.origin.x;
	CGFloat y = rect.origin.y;
	CGFloat height = rect.size.height;
	CGFloat width = rect.size.width;
	CGFloat length = (width > height) ? height : width;
	CGFloat mid = length * 5 / 10;
	CGFloat spacing = length / 20;
	CGFloat radius = (mid - spacing);
	CGFloat miniRadius = spacing;
	CGFloat borderCurveRadius = spacing;
	
	PfmBezierPath *backPath = PfmBezierPathWithRoundedRect(rect, borderCurveRadius);
	
	if (_backgroundColor)
	{
		[_backgroundColor setFill];
		[backPath fill];
	}
	
	if (_borderColor)
	{
		[_borderColor set];
		[backPath setLineWidth:1.0f];
		[backPath stroke];
	}
	
	if (_card)
	{
		// this is for where we draw the number & oval in the bottom right
		CGRect bottomRightRect = CGRectMake(
										x + width - 5 * spacing,
										y + height - mid + 4 * spacing,
										mid - 7 * spacing,
										mid - 6 * spacing);
		
		// this is for where we draw the number & oval in the top left
		CGRect topLeftRect = CGRectMake(
											x + 2 * spacing,
											y + 2 * spacing,
											mid - 7 * spacing,
											mid - 6 * spacing);
		
		if (_foregroundColor)
		{
			[_foregroundColor setFill];
			
			// top left points go clockwise, starting at bottom left of figure, except topLeft0 which is the "middle" point in the square that we use
			// for the circle at the top left corner
			CGPoint topLeft0 = CGPointMake(x + width - spacing - miniRadius, y + height - spacing - miniRadius);
			CGPoint topLeft1 = CGPointMake(x + width - spacing, y + height - mid);
			CGPoint topLeft2 = CGPointMake(topLeft1.x, y + height - spacing - miniRadius);
			CGPoint topLeft3 = CGPointMake(topLeft0.x + (cos(M_PI / 6.0f) * miniRadius), topLeft0.y + (sin(M_PI / 6.0f) * miniRadius));
			CGPoint topLeft4 = CGPointMake(topLeft0.x + (sin(M_PI / 6.0f) * miniRadius), topLeft0.y + (cos(M_PI / 6.0f) * miniRadius));
			CGPoint topLeft5 = CGPointMake(x + width - spacing - miniRadius, y + height - spacing);
			CGPoint topLeft6 = CGPointMake(x + width - mid, topLeft5.y);
			CGPoint topLeft7 = CGPointMake(x + width - spacing - (cos(M_PI / 6.0f) * radius), y + height - spacing - (sin(M_PI / 6.0f) * radius));
			CGPoint topLeft8 = CGPointMake(x + width - spacing - (sin(M_PI / 6.0f) * radius), y + height - spacing - (cos(M_PI / 6.0f) * radius));
			
			// this is the top left blue area.
			PfmBezierPath *topLeftPath = [[[PfmBezierPath alloc] init] autorelease];
			[topLeftPath moveToPoint:topLeft1];
			PfmBezierPathAddLineToPoint(topLeftPath, topLeft2);
			PfmBezierPathAddCurveToPoint(topLeftPath, topLeft5, topLeft3, topLeft4);
			PfmBezierPathAddLineToPoint(topLeftPath ,topLeft6);
			PfmBezierPathAddCurveToPoint(topLeftPath, topLeft1, topLeft7, topLeft8);
			[topLeftPath closePath];
			[topLeftPath fill];
			
			// bottom right points go clockwise, starting at bottom left of figure, except bottomRight0 which is the "middle" point in the
			// square that we use for circle at the bottom right corner
			CGPoint bottomRight0 = CGPointMake(x + spacing + miniRadius, y + spacing + miniRadius);
			CGPoint bottomRight1 = CGPointMake(x + mid, y + spacing);
			CGPoint bottomRight2 = CGPointMake(x + spacing + (cos(M_PI / 6.0f) * radius), y + spacing + (sin(M_PI / 6.0f) * radius));
			CGPoint bottomRight3 = CGPointMake(x + spacing + (sin(M_PI / 6.0f) * radius), y + spacing + (cos(M_PI / 6.0f) * radius));
			CGPoint bottomRight4 = CGPointMake(x + spacing, y + mid);
			CGPoint bottomRight5 = CGPointMake(bottomRight4.x, y + spacing + miniRadius);
			CGPoint bottomRight6 = CGPointMake(bottomRight0.x - (cos(M_PI / 6.0f) * miniRadius), bottomRight0.y - (sin(M_PI / 6.0f) * miniRadius));
			CGPoint bottomRight7 = CGPointMake(bottomRight0.x - (sin(M_PI / 6.0f) * miniRadius), bottomRight0.y - (cos(M_PI / 6.0f) * miniRadius));
			CGPoint bottomRight8 = CGPointMake(x + spacing + miniRadius, bottomRight1.y);
			
			PfmBezierPath *bottomRightPath = [[[PfmBezierPath alloc] init] autorelease];
			[bottomRightPath moveToPoint:bottomRight1];
			PfmBezierPathAddCurveToPoint(bottomRightPath, bottomRight4, bottomRight2, bottomRight3);
			PfmBezierPathAddLineToPoint(bottomRightPath, bottomRight5);
			PfmBezierPathAddCurveToPoint(bottomRightPath, bottomRight8, bottomRight6, bottomRight7);
			[bottomRightPath closePath];
			[bottomRightPath fill];
		}
		
		if (_backgroundColor)
		{
			[_backgroundColor setFill];
			PfmBezierPath *topLeftCircle = [PfmBezierPath bezierPathWithOvalInRect:topLeftRect];
			PfmBezierPath *bottomRightCircle = [PfmBezierPath bezierPathWithOvalInRect:bottomRightRect];
			
			[topLeftCircle fill];
			[bottomRightCircle fill];
		}
		
		if (_foregroundColor)
		{
			NSString *shortString = [self shortNumberString];
			NSMutableDictionary *smallTextAttr = [NSMutableDictionary dictionaryWithCapacity:5];
			[smallTextAttr setValue:[PfmFont fontWithName:@"Helvetica" size:(length / 6)] forKey:NSFontAttributeName];
			[smallTextAttr setValue:_foregroundColor forKey:NSForegroundColorAttributeName];
			[self drawString:shortString centeredIn:bottomRightRect withAttributes:smallTextAttr];
			[self drawString:shortString centeredIn:topLeftRect withAttributes:smallTextAttr];
			
			NSString *longString = [self numberString];
			
			NSUInteger largeFontSize = (length * 2 / 3);
			if ([longString length] > 2)
			{
				largeFontSize = (length * 2 / 5);
			}
			
			NSMutableDictionary *longTextAttr = [NSMutableDictionary dictionaryWithCapacity:5];
			[longTextAttr setValue:[PfmFont fontWithName:@"Helvetica" size:largeFontSize] forKey:NSFontAttributeName];
			[longTextAttr setValue:_foregroundColor forKey:NSForegroundColorAttributeName];
			[self drawString:longString centeredIn:rect withAttributes:longTextAttr];
		}
		
		if (_selected)
		{
			if (_selectedColor)
			{
				[_selectedColor setFill];
				[backPath fill];
			}
			if (_selectedBorderColor)
			{
				[_selectedBorderColor set];
				[backPath setLineWidth:2.0f];
				[backPath stroke];
			}
		}
		else if (_unavailable)
		{
			[_unavailableColor setFill];
			[backPath fill];
		}
	}
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


@end
