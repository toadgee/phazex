//
//  PfmColor.m
//  phazex
//
//  Created by toddha on 11/18/16.
//
//

#if TARGET_OS_IOS
#import <UIKit/UIKit.h>
#elif TARGET_OS_MAC
#import <Cocoa/Cocoa.h>
#endif

#import "PfmColor.h"

PfmColor *PfmColorWithRGB(CGFloat red, CGFloat green, CGFloat blue, CGFloat alpha)
{
#if TARGET_OS_IOS
	UIColor *color = [UIColor colorWithRed:red green:green blue:blue alpha:alpha];
#elif TARGET_OS_MAC
	NSColor *color = [NSColor colorWithDeviceRed:red green:green blue:blue alpha:alpha];
#else
	#error No target set
#endif
return color;
}
