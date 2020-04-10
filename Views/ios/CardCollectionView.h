//
//  CardCollectionView.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import "CardCollection.h"

@interface PXCardCollectionView : UIView
@property (readwrite, assign, nonatomic) PXCard *selectedCard;

- (void)setHovers:(BOOL)hovers;
- (void)setCardCollection:(PXCardCollection *)cardCollection;
@end
