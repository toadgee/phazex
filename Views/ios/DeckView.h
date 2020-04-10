//
//  DeckView.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import <UIKit/UIKit.h>

@class PXDeckView;
@protocol PXDeckViewDelegate <NSObject>
- (void)deckView:(PXDeckView *)deckView beganTouches:(NSSet<UITouch *> *)touches;
- (void)deckView:(PXDeckView *)deckView movedTouches:(NSSet<UITouch *> *)touches;
- (void)deckView:(PXDeckView *)deckView endedTouches:(NSSet<UITouch *> *)touches;
@end
@interface PXDeckView : UIImageView
@property (readwrite, assign, nonatomic) id<PXDeckViewDelegate> delegate;
@end
