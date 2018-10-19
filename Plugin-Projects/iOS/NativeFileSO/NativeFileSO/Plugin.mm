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
    
    const BOOL pluginIsFileLoaded() {
        return [NativeFileOpenURLBuffer instance].isFileOpened;
    }
    
    const BOOL pluginIsTextFile() {
        return [NativeFileOpenURLBuffer instance].isTextFile;
    }
    
    const void* pluginGetData() {
        return [[NativeFileOpenURLBuffer instance] data].bytes;
    }
    
    const unsigned long pluginGetDataByteCount() {
        return [[NativeFileOpenURLBuffer instance] data].length;
    }
    
    const char* pluginGetStringContents() {
        return [[[NativeFileOpenURLBuffer instance] stringContents] UTF8String];
    }
    
    const char* pluginGetFilename() {
        return [[[NativeFileOpenURLBuffer instance] filename] UTF8String];
    }
    
    const char* pluginGetExtension() {
        return [[[NativeFileOpenURLBuffer instance] extension] UTF8String];
    }
    
    void pluginResetLoadedFile() {
        [[NativeFileOpenURLBuffer instance] reset];
    }
    
    
    const char* pluginOpenFile(const char* utis) {
        
        return [NativeFileSO fileOpen:[NSString stringWithUTF8String:utis]];
    }
    
    void pluginSaveFile(const char* srcPath,
                   const char* name) {
        
        [NativeFileSO fileSave:[NSString stringWithUTF8String:srcPath]
                     //extension:[NSString stringWithUTF8String:extension]
                          name:[NSString stringWithUTF8String:name]];
    }
}
