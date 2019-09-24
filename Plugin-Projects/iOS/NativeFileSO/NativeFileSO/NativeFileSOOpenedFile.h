//
//  NativeFileSOOpenedFile.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 28.10.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#ifndef NativeFileSOOpenedFile_h
#define NativeFileSOOpenedFile_h

@interface NativeFileSOOpenedFile : NSObject

@property (readonly, nonatomic, strong) NSString *filename;
@property (readonly, nonatomic, strong) NSData *data;

-(id)initWithFilename:(NSString *)filename
                 data:(NSData *)data;

@end

#endif /* NativeFileSOOpenedFile_h */
