//
//  NativeFileOpenURLBuffer.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard.
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

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
