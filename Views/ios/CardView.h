//
//  CardView.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import <UIKit/UIKit.h>
#import "Card.h"

@class PXCardView;
@protocol PXCardViewDelegate <NSObject>
- (void)cardView:(PXCardView *)cardView beganTouches:(NSSet<UITouch *> *)touches;
- (void)cardView:(PXCardView *)cardView movedTouches:(NSSet<UITouch *> *)touches;
- (void)cardView:(PXCardView *)cardView endedTouches:(NSSet<UITouch *> *)touches;
@end

@interface PXCardView : UIView
@property (readwrite, assign, nonatomic) PXCard *card;
@property (readwrite, assign, nonatomic) id<PXCardViewDelegate> delegate;
@end
