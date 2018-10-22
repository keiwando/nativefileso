//
//  NativeFileSO.h
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 19.09.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#ifndef NativeFileSO_h
#define NativeFileSO_h

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>
#import "UnityCallbackFunction.h"

@interface NativeFileSO : NSObject

+ (void) setCallback:(UnityCallbackFunction)callback;

+ (void) fileOpen:(NSString *)extensions;

+ (void) fileSave:(NSString *)extension
                     name:(NSString *)name;

@end

#endif /* NativeFileSO_h */
