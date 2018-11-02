//
//  NativeFileOpenURLBuffer.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//
//

#import "NativeFileOpenURLBuffer.h"

@interface NativeFileOpenURLBuffer () {
    
    NSMutableArray<NativeFileSOOpenedFile *> *_openedFiles;
}

@end

@implementation NativeFileOpenURLBuffer

static NativeFileOpenURLBuffer *_nativeFileOpenURLInstance;
static UnityCallbackFunction _nativeFileSOUnityCallback;

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
    if (_openedFiles != nil) {
        [_openedFiles removeAllObjects];
    } else {
        _openedFiles = [[NSMutableArray alloc] init];
    }
}

-(void)loadBufferFromURLs:(NSArray<NSURL *> *)URLs {
    
    [_openedFiles removeAllObjects];
    
    for (int i = 0; i < URLs.count; i++) {
        [self addToBufferFromURL:URLs[i]];
    }
    
    if ([_openedFiles count] > 0 && _nativeFileSOUnityCallback) {
        
        dispatch_async(dispatch_get_main_queue(), ^(void){
            _nativeFileSOUnityCallback();
        });
    }
}

-(void)addToBufferFromURL:(NSURL *)URL {
    
    NativeFileSOOpenedFile *openedFile = [self loadFileFromURL:URL];
    [_openedFiles addObject:openedFile];
}

-(NativeFileSOOpenedFile *)loadFileFromURL:(NSURL *)URL {
    
    NSData *data = [NSData dataWithContentsOfURL:URL];
    NSString *filename = URL.lastPathComponent ? : @"";
    
    NativeFileSOOpenedFile *file = [[NativeFileSOOpenedFile alloc] initWithFilename:filename data:data];
    
    return file;
}

-(void)setCallback:(UnityCallbackFunction) callback {
    _nativeFileSOUnityCallback = callback;
}

-(void)sendCallback {
    if (_nativeFileSOUnityCallback) {
        dispatch_async(dispatch_get_main_queue(), ^(void){
            _nativeFileSOUnityCallback();
        });
    }
}

-(int)getNumberOfOpenedFiles {
    return (int)_openedFiles.count;
}

-(NativeFileSOOpenedFile *)getOpenedFileAtIndex:(int)index {
    if (index >= _openedFiles.count) {
        return nil;
    }
    return _openedFiles[index];
}

- (void)documentPicker:(UIDocumentPickerViewController *)controller didPickDocumentsAtURLs:(NSArray<NSURL *> *)urls {
    
    [self loadBufferFromURLs:urls];
}

- (void)documentPickerWasCancelled:(UIDocumentPickerViewController *)controller {
    [self sendCallback];
}

@end
