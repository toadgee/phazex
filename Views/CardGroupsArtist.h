//
//  CardGroupsArtist.h
//  phazex
//
//  Created by toddha on 12/22/16.
//
//

#import "PfmView.h"

@class PXGroup;
@class PXCardGroupsArtist;

@protocol PXCardGroupsArtistDelegate <NSObject>
- (CGRect)boundsForArtist:(PXCardGroupsArtist *)artist;
- (NSArray<PfmView *> *)subviewsForArtist:(PXCardGroupsArtist *)artist;
- (PfmView *)cardGroupsArtist:(PXCardGroupsArtist *)artist stackViewForGroup:(PXGroup *)group;
- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist updateSubviews:(NSArray<PfmView *> *)subviews;
- (void)cardGroupsArtist:(PXCardGroupsArtist *)artist groupViewNeedsDisplay:(PfmView *)groupView;
@end

@interface PXCardGroupsArtist : NSObject
@property (readonly, assign, nonatomic) int ptr;
@property (readwrite, assign, nonatomic) id<PXCardGroupsArtistDelegate> delegate;
@property (readwrite, retain, nonatomic) NSArray<PXGroup *> *groups;
@property (readonly, retain, nonatomic) PXGroup *currentGroup;

- (void)nextGroup;
- (void)previousGroup;

-(void)drawRect:(CGRect)rect;
@end
