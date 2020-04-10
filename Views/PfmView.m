//
//  PfmView.m
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

#import "PfmView.h"

id PfmViewGetAnimator(PfmView *view)
{
#if TARGET_OS_IOS
	return [view layer];
#elif TARGET_OS_MAC
	return [view animator];
#endif
}
