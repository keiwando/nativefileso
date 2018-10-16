//
//  Plugin.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 14.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeFileSO.h"
#import "NativeFileOpenURLBuffer.h"

extern "C" {
    
    const char* pluginOpenFile(const char* extensions) {
        
        return [NativeFileSO fileOpen:[NSString stringWithUTF8String:extensions]];
    }
    
    void pluginSaveFile(const char* srcPath,
                   const char* name) {
        
        [NativeFileSO fileSave:[NSString stringWithUTF8String:srcPath]
                     //extension:[NSString stringWithUTF8String:extension]
                          name:[NSString stringWithUTF8String:name]];
    }
    
    const char* pluginGetOpenURL() {
        //return [[[NativeFileOpenURLBuffer instance] URL] UTF8String];
        //return [NSString stringWith]; [[NativeFileOpenURLBuffer instance] data]
        return [[[NativeFileOpenURLBuffer instance] URLContents] UTF8String];
    }
}
