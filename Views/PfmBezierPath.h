//
//  PfmBezierPath.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#if TARGET_OS_IOS
typedef UIBezierPath PfmBezierPath;
#elif TARGET_OS_MAC
typedef NSBezierPath PfmBezierPath;
#else
#error No platform
#endif

PfmBezierPath *PfmBezierPathWithRoundedRect(CGRect rect, CGFloat cornerRadius);
void PfmBezierPathAddCurveToPoint(PfmBezierPath *path, CGPoint point, CGPoint controlPoint1, CGPoint controlPoint2);
void PfmBezierPathAddLineToPoint(PfmBezierPath *path, CGPoint point);
void PfmBezierPathFillRect(CGRect rect);
void PfmBezierPathStrokeRect(CGRect rect);
