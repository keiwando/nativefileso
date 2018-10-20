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
#import "UnityCallbackFunction.h"

extern "C" {
    
    void pluginSetCallback(UnityCallbackFunction callback) {
        [[NativeFileOpenURLBuffer instance] setCallback:callback];
    }
    
    const BOOL pluginIsFileLoaded() {
        return [NativeFileOpenURLBuffer instance].isFileOpened;
    }
    
    const void* pluginGetData() {
        return [[NativeFileOpenURLBuffer instance] data].bytes;
    }
    
    const unsigned long pluginGetDataByteCount() {
        return [[NativeFileOpenURLBuffer instance] data].length;
    }
    
    const char* pluginGetFilename() {
        return [[[NativeFileOpenURLBuffer instance] filename] UTF8String];
    }
    
    void pluginResetLoadedFile() {
        [[NativeFileOpenURLBuffer instance] reset];
    }
    
    
    void pluginOpenFile(const char* utis) {
        
        [NativeFileSO fileOpen:[NSString stringWithUTF8String:utis]];
    }
    
    void pluginSaveFile(const char* srcPath,
                   const char* name) {
        
        [NativeFileSO fileSave:[NSString stringWithUTF8String:srcPath]
                          name:[NSString stringWithUTF8String:name]];
    }
}
