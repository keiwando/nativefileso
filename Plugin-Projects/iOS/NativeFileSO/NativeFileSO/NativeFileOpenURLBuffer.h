//
//  NativeFileOpenURLBuffer.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#ifndef NativeFileOpenURLBuffer_h
#define NativeFileOpenURLBuffer_h

#import <Foundation/Foundation.h>

@interface NativeFileOpenURLBuffer : NSObject

//@property (nonatomic, strong) NSData *data;
@property (nonatomic, strong) NSString *URLContents;

+(NativeFileOpenURLBuffer *)instance;
-(void)reset;
-(void)loadBufferFromURL:(NSURL *)URL;

@end

#endif /* NativeFileOpenURLBuffer_h */
