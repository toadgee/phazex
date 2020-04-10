//
//  PfmBezierPath.m
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

#import "PfmBezierPath.h"

PfmBezierPath *PfmBezierPathWithRoundedRect(CGRect rect, CGFloat cornerRadius)
{
#if TARGET_OS_IOS
	return [PfmBezierPath bezierPathWithRoundedRect:rect cornerRadius:cornerRadius];
#elif TARGET_OS_MAC
	return [PfmBezierPath bezierPathWithRoundedRect:rect xRadius:cornerRadius yRadius:cornerRadius];
#endif
}

void PfmBezierPathAddCurveToPoint(PfmBezierPath *path, CGPoint point, CGPoint controlPoint1, CGPoint controlPoint2)
{
#if TARGET_OS_IOS
	[path addCurveToPoint:point controlPoint1:controlPoint1 controlPoint2:controlPoint2];
#elif TARGET_OS_MAC
	[path curveToPoint:point controlPoint1:controlPoint1 controlPoint2:controlPoint2];
#endif
}

void PfmBezierPathAddLineToPoint(PfmBezierPath *path, CGPoint point)
{
#if TARGET_OS_IOS
	[path addLineToPoint:point];
#elif TARGET_OS_MAC
	[path lineToPoint:point];
#endif
}

void PfmBezierPathFillRect(CGRect rect)
{
#if TARGET_OS_IOS
	PfmBezierPath *path = [PfmBezierPath bezierPathWithRect:rect];
	[path fill];
#elif TARGET_OS_MAC
	[PfmBezierPath fillRect:rect];
#endif
}

void PfmBezierPathStrokeRect(CGRect rect)
{
#if TARGET_OS_IOS
	PfmBezierPath *path = [PfmBezierPath bezierPathWithRect:rect];
	[path stroke];
#elif TARGET_OS_MAC
	[PfmBezierPath strokeRect:rect];
#endif

}
