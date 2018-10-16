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
    //self.data = [NSData new];
    self.URLContents = @"";
}

-(void)loadBufferFromURL:(NSURL *)URL {
    
    //self.data = [NSData dataWithContentsOfURL:URL];
    self.URLContents = [NSString stringWithContentsOfURL:URL encoding:NSUTF8StringEncoding error:nil];
    
    NSLog(self.URLContents);
    
    if (self.URLContents == nil) {
        self.URLContents = @"";
    }
    
    NSLog(self.URLContents);
}

@end
