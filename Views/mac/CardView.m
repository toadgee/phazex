//
//  CardView.m
//  phazex
//
//  Created by toddha on 10/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardView.h"
#import "CardArtist.h"

@interface CardView () < PXCardArtistDelegate >
{
	PXCardArtist *_artist;
}
@end

@implementation CardView

-(BOOL)wantsDefaultClipping
{
	return NO;
}

- (id)initWithFrame:(NSRect)frameRect
{
	self = [super initWithFrame:frameRect];
	
	if (self)
	{
		_artist = [PXCardArtist new];
		[self setWantsLayer:YES];
	}
	
	return self;
}

- (BOOL)isFlipped
{
	return YES; // to match iOS
}

-(void)dealloc
{
	[_artist release];
	[super dealloc];
}

- (void)setSelected:(BOOL)selected { [_artist setIsSelected:selected]; }
- (BOOL)isSelected { return [_artist isSelected]; }
- (void)setUnavailable:(BOOL)unavailable { [_artist setIsUnavailable:unavailable]; }
- (BOOL)isUnavailable { return [_artist isUnavailable]; }
- (PXCard *)card { return [_artist card]; }
- (void)setCard:(PXCard *)card { [_artist setCard:card]; }

- (void)updateViewForArtist:(PXCardArtist *)artist
{
	[self setNeedsDisplay:YES];
}

- (void)drawRect:(NSRect)rect
{
	[_artist drawInRect:rect];
}

-(NSString *)description
{
	NSMutableString *str = [NSMutableString stringWithCapacity:11];
	
	switch (CardColor([_artist card]))
	{
		case 0:
		[str appendString:@"Red "];
		break;
		
		case 1:
		[str appendString:@"Yellow "];
		break;
		
		case 2:
		[str appendString:@"Green "];
		break;
		
		case 3:
		[str appendString:@"Blue "];
		break;
	}
	
	switch (CardNumber([_artist card]))
	{
		case 0:
		[str appendString:@"Wild"];
		break;
		
		case 13:
		[str appendString:@"Skip"];
		break;
		
		default:
		[str appendFormat:@"%d", CardNumber([_artist card])];
		break;
	}
	
	return str;
}

@end
