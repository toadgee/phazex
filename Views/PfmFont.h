//
//  PfmFont.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#if TARGET_OS_IOS
typedef UIFont PfmFont;
#elif TARGET_OS_MAC
typedef NSFont PfmFont;
#else
#error No platform
#endif
