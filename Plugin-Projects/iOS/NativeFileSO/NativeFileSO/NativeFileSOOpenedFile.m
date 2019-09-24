//
//  NativeFileSOOpenedFile.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 29.10.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

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
