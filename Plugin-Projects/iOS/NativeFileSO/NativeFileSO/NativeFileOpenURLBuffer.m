//
//  NativeFileOpenURLBuffer.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//
// Based on: https://github.com/eppz/Unity.Blog.Override_App_Delegate/blob/3.3/iOS/Override_iOS/Override_iOS/DeepLink.m
//

#import "NativeFileOpenURLBuffer.h"

__strong NativeFileOpenURLBuffer *_nativeFileOpenURLInstance;

@implementation NativeFileOpenURLBuffer

+(void)load {
    _nativeFileOpenURLInstance = [NativeFileOpenURLBuffer new];
}

+(NativeFileOpenURLBuffer *)instance {
    return _nativeFileOpenURLInstance;
}

-(instancetype)init {
    self = [super init];
    if (self) [self reset];
    return self;
}

-(void)reset {
    self.data = [NSData new];
    self.stringContents = @"";
    self.filename = @"";
    self.isTextFile = NO;
    self.isFileOpened = NO;
}

-(void)loadBufferFromURL:(NSURL *)URL {
    
    self.data = [NSData dataWithContentsOfURL:URL];
    self.stringContents = [NSString stringWithContentsOfURL:URL encoding:NSUTF8StringEncoding error:nil];
    
    self.extension = URL.pathExtension ? : @"";
    self.filename = URL.lastPathComponent ? : @"";
    
    self.isTextFile = self.stringContents != nil;
    self.stringContents = self.stringContents ? : @"";
    
    self.isFileOpened = self.data != nil;
    self.data = self.data ? : [NSData new];
    
    NSLog(@"Loaded file from buffer: %s", self.isFileOpened ? @"true" : @"false");
}

- (void)documentPicker:(UIDocumentPickerViewController *)controller didPickDocumentsAtURLs:(NSArray<NSURL *> *)urls {
    
    NSURL *url = urls.firstObject;
    if (url == nil) {
        return;
    }
    [self loadBufferFromURL:url];
}

@end
