//
//  CardGroupsView.h
//  phazex
//
//  Created by toddha on 12/22/16.
//
//

#import <UIKit/UIKit.h>
#import "Group.h"

@class PXCardGroupsView;
@protocol PXCardGroupsViewDelegate <NSObject>
//- (void)cardView:(PXCardView *)cardView beganTouches:(NSSet<UITouch *> *)touches;
//- (void)cardView:(PXCardView *)cardView movedTouches:(NSSet<UITouch *> *)touches;
//- (void)cardView:(PXCardView *)cardView endedTouches:(NSSet<UITouch *> *)touches;
@end

@interface PXCardGroupsView : UIView
@property (readwrite, assign, nonatomic) id<PXCardGroupsViewDelegate> delegate;

@property (readwrite, retain, nonatomic) NSArray<PXGroup *> *groups;
@property (readonly, retain, nonatomic) PXGroup *currentGroup;

@end
