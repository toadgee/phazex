//
//  CardGroupsArtist.m
//  phazex
//
//  Created by toddha on 12/22/16.
//
//

#import "CardGroupsArtist.h"
#import "PfmColor.h"
#import "PfmBezierPath.h"
#import "PfmView.h"

@interface PXCardGroupsArtist ()
{
	int _ptr;
	NSArray<PXGroup *> *_groups;
}

@end

@implementation PXCardGroupsArtist

- (instancetype)init {
	self = [super init];
	if (self) {
	}
	
	return self;
}

- (void)dealloc
{
	[_groups release];
	[super dealloc];
}

- (void)nextGroup
{
	int count = (int)[_groups count];
	if (count == 0) return;
	_ptr = (_ptr + 1) % count;
	[self recalculatePositions:YES];
}

- (void)previousGroup
{
	int count = (int)[_groups count];
	if (count == 0) return;
	_ptr = (_ptr - 1) % count;
	[self recalculatePositions:YES];
}

-(PXGroup *)currentGroup
{
	return [_groups objectAtIndex:_ptr];
}

-(NSArray<PXGroup *> *)groups
{
	return [NSArray arrayWithArray:_groups];
}

-(void)setGroups:(NSArray<PXGroup *> *)groups
{
	[_groups autorelease];
	_groups = [groups retain];
	
	// cache the groups count since we use it a bit
	int count = (int)[_groups count];
	
	if (_groups == nil || count == 0)
	{
		return;
	}
	
	if (_ptr >= count)
	{
		_ptr = 0;
	}
	
	// start at the first group (as to make it centered)
	NSMutableArray<PfmView *> *newSubviews = [NSMutableArray arrayWithCapacity:count];
	for (PXGroup *group in groups)
	{
		PfmView *view = [[self delegate] cardGroupsArtist:self stackViewForGroup:group];
		[newSubviews addObject:view];
	}
	
	[[self delegate] cardGroupsArtist:self updateSubviews:newSubviews];
	[self recalculatePositions:NO];
}

-(void)recalculatePositions:(BOOL)useAnimation
{
	NSArray<PfmView *> *groupViews = [[self delegate] subviewsForArtist:self];
	
	if ([groupViews count] == 0)
		return;
	
	CGRect bounds = [[self delegate] boundsForArtist:self];
	CGFloat xCenter = bounds.size.width / 2;
	CGFloat yCenter = bounds.size.height / 2;
	
	// the max and min widths and heights of the subviews
	CGFloat maxWidth = bounds.size.width / 3;
	CGFloat maxHeight = bounds.size.height * 2 / 3;
	CGFloat minWidth = bounds.size.width / 8;
	CGFloat minHeight = bounds.size.height / 6;
	CGFloat deltaWidth = maxWidth - minWidth;
	CGFloat deltaHeight = maxHeight - minHeight;
	
	// calculate the bottom most and top most points
	CGFloat bottomMost = (bounds.size.height * 3 / 8) - (maxHeight / 2);
	CGFloat topMost = (bounds.size.height * 5 / 6) - (minHeight / 2);
	
	// a is length from middle of ellipse to furthest out on the x-axis of ellipse
	CGFloat a = bounds.size.width - ((maxWidth + minWidth) / 2) - xCenter;
	
	// b is length from middle of ellipse to furthest out on the y-axis of ellipse
	CGFloat b = topMost - yCenter;
	
	// this is the angle for each item
	CGFloat anglePerItem = 2 * M_PI / [groupViews count];
	
	int i = _ptr;
	for (PfmView *groupView in groupViews)
	{
		// note that these are the center points, we will need to adjust the height/width afterwards
		CGFloat x = a * cos(anglePerItem * i - (M_PI / 2)) + xCenter;
		CGFloat y = b * sin(anglePerItem * i - (M_PI / 2)) + yCenter;
		
		// percentage is based off of the distance from the front in Y
		CGFloat pct = (y - topMost) / (bottomMost - topMost);
		
		// calculate the width & height of the group
		CGFloat width = minWidth + deltaWidth * pct;
		CGFloat height = minHeight + deltaHeight * pct;
		
		// adjust the X & Y positions so that they look centered
		x -= (width / 2);
		y -= (height / 2);
		
		// create the rect, move the item
		CGRect groupViewFrame = CGRectMake(x, y, width, height);
		id layer = useAnimation ? PfmViewGetAnimator(groupView) : groupView;
		[layer setFrame:groupViewFrame];
		[[self delegate] cardGroupsArtist:self groupViewNeedsDisplay:groupView];
		i++;
	}
}

-(void)drawRect:(CGRect)rect
{
	[[PfmColor blackColor] set];
	PfmBezierPathStrokeRect([[self delegate] boundsForArtist:self]);
}



@end
