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
#import "NativeFileSOOpenedFile.h"

@interface NativeFileOpenURLBuffer : NSObject <UIDocumentPickerDelegate>

+(NativeFileOpenURLBuffer *)instance;
-(void)reset;
-(void)loadBufferFromURLs:(NSArray<NSURL *> *)URLs;
-(void)addToBufferFromURL:(NSURL *)URL;
-(void)setCallback:(UnityCallbackFunction)callback;
-(void)sendCallback;
-(int)getNumberOfOpenedFiles;
-(NativeFileSOOpenedFile *)getOpenedFileAtIndex:(int)index;

@end

#endif /* NativeFileOpenURLBuffer_h */
