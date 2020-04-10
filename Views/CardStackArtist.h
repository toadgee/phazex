//
//  CardStackArtist.h
//  phazex
//
//  Created by Todd Harris on 11/19/16.
//
//

#import "Card.h"
#import "PfmView.h"

@class PXCardCollection;

@class PXCardStackArtist;
@protocol PXCardStackArtistDelegate <NSObject>
- (void)cardStackArtistUpdateView:(PXCardStackArtist *)artist;
- (void)cardStackArtistUpdateTrackingArea:(PXCardStackArtist *)artist;
- (void)cardStackArtist:(PXCardStackArtist *)artist updateSubviewsForCollection:(PXCardCollection *)collection;
- (CGRect)cardStackArtistBounds:(PXCardStackArtist *)artist;
- (NSArray<PfmView *> *)cardStackArtistSubviews:(PXCardStackArtist *)artist;
- (PXCard *)cardStackArtist:(PXCardStackArtist *)artist cardForView:(PfmView *)view;
@end

@interface PXCardStackArtist : NSObject
@property (readwrite, assign, nonatomic) id<PXCardStackArtistDelegate> delegate;
@property (readwrite, retain) PXCardCollection *cards;
@property (readwrite, assign, nonatomic) int hoveringCardId;

@property (readwrite, assign, nonatomic, getter=isFlipped) BOOL flipped;
@property (readwrite, assign, nonatomic, getter=hasFanEffect) BOOL fanEffect;
@property (readwrite, assign, nonatomic, getter=doesHover) BOOL hovers;
@property (readwrite, assign, nonatomic, getter=isStacked) BOOL stacked;
@property (readwrite, assign, nonatomic, getter=isEnabled) BOOL enabled;
@property (readwrite, assign, nonatomic, getter=isVerticallyCentered) BOOL verticallyCentered;
@property (readwrite, assign, nonatomic, getter=isHorizontallyCentered) BOOL horizontallyCentered;

-(void)recalculatePositions:(BOOL)useAnimation;

@end
