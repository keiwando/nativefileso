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

@interface NativeFileSO : NSObject

+ (const char *) fileOpen:(NSArray<NSString *> *)fileExtensions;
+ (const char *) fileSave:(NSString *)originalPath;

@end

#endif /* NativeFileSO_h */
