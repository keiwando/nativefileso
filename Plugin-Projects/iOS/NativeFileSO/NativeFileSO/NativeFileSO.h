//
//  NativeFileSO.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 14.10.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#import <Foundation/Foundation.h>
#import "NativeFileOpenURLBuffer.h"
#import "UIKit/UIKit.h"

@interface NativeFileSO : NSObject

+ (void) fileOpen:(NSString *)UTIs
   allowsMultiple:(bool)allowsMultiple;

+ (void) fileSave:(NSString *)srcPath
                //extension:(NSString *)extension
             name:(NSString *)name;

@end
