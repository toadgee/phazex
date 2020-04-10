//
//  CardView.h
//  phazex
//
//  Created by toddha on 10/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <QuartzCore/CoreAnimation.h>
#import "Card.h"

@interface CardView : NSView

@property (readwrite, assign, nonatomic, getter=isSelected) BOOL selected;
@property (readwrite, assign, nonatomic, getter=isUnavailable) BOOL unavailable;
@property (readwrite, assign, nonatomic) PXCard *card;



@end


