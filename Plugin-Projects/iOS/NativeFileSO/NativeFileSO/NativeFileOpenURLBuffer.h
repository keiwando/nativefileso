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
#import <UIKit/UIKit.h>
#import "UnityCallbackFunction.h"

@interface NativeFileOpenURLBuffer : NSObject <UIDocumentPickerDelegate>

@property (nonatomic) BOOL isFileOpened;
@property (nonatomic, strong) NSData *data;
@property (nonatomic, strong) NSString *filename;

+(NativeFileOpenURLBuffer *)instance;
-(void)reset;
-(void)loadBufferFromURL:(NSURL *)URL;
-(void)setCallback:(UnityCallbackFunction)callback;

@end

#endif /* NativeFileOpenURLBuffer_h */
