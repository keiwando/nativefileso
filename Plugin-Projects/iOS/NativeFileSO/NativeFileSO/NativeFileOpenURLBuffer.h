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

@interface NativeFileOpenURLBuffer : NSObject <UIDocumentPickerDelegate>

@property (nonatomic) BOOL isFileOpened;
@property (nonatomic) BOOL isTextFile;
@property (nonatomic, strong) NSData *data;
@property (nonatomic, strong) NSString *stringContents;
@property (nonatomic, strong) NSString *filename;
@property (nonatomic, strong) NSString *extension;

+(NativeFileOpenURLBuffer *)instance;
-(void)reset;
-(void)loadBufferFromURL:(NSURL *)URL;

@end

#endif /* NativeFileOpenURLBuffer_h */
