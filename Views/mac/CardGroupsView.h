//
//  CardGroupsView.h
//  phazex
//
//  Created by toddha on 12/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "CardStackView.h"
#import "Group.h"

@class CardGroupsView;
@protocol CardGroupViewDelegate <NSObject>
-(void)cardGroupsView:(CardGroupsView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardGroup:(CardStackView *)cardGroup onCardView:(CardView *)cardView isMainCardGroup:(BOOL)isMainCardGroup movesGroups:(BOOL*)movesGroups;
@end

@interface CardGroupsView : NSView {
	id<CardGroupViewDelegate> _delegate;
}

@property (readwrite, assign, nonatomic) id<CardGroupViewDelegate> delegate;
@property (readwrite, retain, nonatomic) NSArray<PXGroup *> *groups;

- (IBAction) nextGroup:(id)sender;
- (IBAction) previousGroup:(id)sender;

-(PXGroup *)currentGroup;


@end
