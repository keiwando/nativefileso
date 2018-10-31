//
//  NativeFileSOOpenedFile.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 29.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeFileSOOpenedFile.h"

@implementation NativeFileSOOpenedFile

@synthesize filename = _filename;
@synthesize data = _data;

-(id)initWithFilename:(NSString *)filename
                 data:(NSData *)data {
    
    self = [super init];
    if (self) {
        _filename = filename;
        _data = data;
    }
    return self;
}

@end
