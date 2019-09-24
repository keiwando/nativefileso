//
//  NativeFileOpenHandler.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#ifndef NativeFileOpenHandler_h
#define NativeFileOpenHandler_h

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface NativeFileOpenHandler : NSObject

- (BOOL)application:(UIApplication *)app
            openURL:(NSURL *)url
            options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options;

- (BOOL)native_file_open_copy_application:(UIApplication *)app
                                  openURL:(NSURL *)url
                                  options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options;

void UnitySendMessage(const char* obj, const char* method, const char* msg);

@end

#endif /* NativeFileOpenHandler_h */
