//
//  PfmView.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#if TARGET_OS_IOS
typedef UIView PfmView;
#elif TARGET_OS_MAC
typedef NSView PfmView;
#else
#error No platform
#endif

id PfmViewGetAnimator(PfmView *view);
