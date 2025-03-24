//
//  NativeFileSO.h
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 19.09.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#ifndef NativeFileSO_h
#define NativeFileSO_h

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>
#import "UnityCallbackFunction.h"

@interface NativeFileSO : NSObject

+ (void) setCallback:(UnityCallbackFunction)callback;

+ (void) fileOpen:(NSString *)extensions
canSelectMultiple:(bool)canSelectMultiple
            title:(NSString *)title
        directory:(NSString *)directory;

+ (const char *) fileOpenSync:(NSString *)extensions
       canSelectMultiple:(bool)canSelectMultiple
                   title:(NSString *)title
               directory:(NSString *)directory;

+ (void) fileSave:(NSString *)extensions
                name:(NSString *)name
                title:(NSString *)title
            directory:(NSString *)directory;


+ (const char *) fileSaveSync:(NSString *)extensions
                   name:(NSString *)name
                  title:(NSString *)title
              directory:(NSString *)directory;

+ (void) freeDynamicMemory;

+ (NSString *) optionalString:(const char *)cString;

@end

#endif /* NativeFileSO_h */
