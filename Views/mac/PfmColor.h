//
//  PfmColor.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#if TARGET_OS_IOS
typedef UIColor PfmColor;
#elif TARGET_OS_MAC
typedef NSColor PfmColor;
#else
#error No platform
#endif

PfmColor *PfmColorWithRGB(CGFloat red, CGFloat green, CGFloat blue, CGFloat alpha);
