//
//  PlayerStatusArtist.h
//  phazex
//
//  Created by toddha on 11/26/16.
//
//

#import <Foundation/Foundation.h>

@class PXPlayerStatusArtist;
@protocol PXPlayerStatusArtistDelegate <NSObject>
- (void)playerStatusArtistsNeedsDisplay:(PXPlayerStatusArtist *)artist;
@end
@interface PXPlayerStatusArtist : NSObject
@property (readwrite, strong, nonatomic) NSArray<PXPlayer *> *players;
@property (readwrite, assign, nonatomic) id<PXPlayerStatusArtistDelegate> delegate;
- (void)drawRect:(CGRect)rect;
@end
