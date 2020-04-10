//
//  CardArtist.h
//  phazex
//
//  Created by toddha on 11/17/16.
//
//

#import "PfmColor.h"
#import "Card.h"

@class PXCardArtist;

@protocol PXCardArtistDelegate <NSObject>
- (void)updateViewForArtist:(PXCardArtist *)artist;
@end

@interface PXCardArtist : NSObject
@property (readwrite, assign) PXCard *card; // TODO
@property (readwrite, retain) PfmColor *backgroundColor;
@property (readwrite, retain) PfmColor *foregroundColor;
@property (readwrite, retain) PfmColor *borderColor;
@property (readwrite, retain) PfmColor *selectedColor;
@property (readwrite, retain) PfmColor *selectedBorderColor;
@property (readwrite, retain) PfmColor *unavailableColor;

@property (readwrite, assign) BOOL isSelected;
@property (readwrite, assign) BOOL isUnavailable;

@property (readwrite, assign, nonatomic) id<PXCardArtistDelegate> delegate;


+(CGFloat)preferredCardWidthForHeight:(CGFloat)height;
+(CGFloat)preferredCardHeightForWidth:(CGFloat)width;

- (void)drawInRect:(CGRect)rect;

-(NSString*) shortNumberString;
-(NSString*) numberString;

@end
