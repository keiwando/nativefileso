//
//  Plugin.m
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 18.09.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>
#import "NativeFileSO.h"
#import "UnityCallbackFunction.h"

extern "C" {
    
    void pluginSetCallback(UnityCallbackFunction callback) {
        [NativeFileSO setCallback:callback];
    }
    
    void pluginOpenFile(const char *extensions,
                        bool canSelectMultiple,
                        const char *title,
                        const char *directory) {

        [NativeFileSO fileOpen:[NativeFileSO optionalString:extensions]
             canSelectMultiple:canSelectMultiple
                         title:[NativeFileSO optionalString:title]
                     directory:[NativeFileSO optionalString:directory]];
    }
    
    const char* pluginOpenFileSync(const char *extensions,
                            bool canSelectMultiple,
                            const char *title,
                            const char *directory) {
        
        return [NativeFileSO fileOpenSync:[NativeFileSO optionalString:extensions]
                        canSelectMultiple:canSelectMultiple
                                    title:[NativeFileSO optionalString:title]
                                directory:[NativeFileSO optionalString:directory]];
    }
    
    void pluginSaveFile(const char *name,
                        const char *extension,
                        const char *title,
                        const char *directory) {
        
        [NativeFileSO fileSave:[NativeFileSO optionalString:extension]
                          name:[NativeFileSO optionalString:name]
                         title:[NativeFileSO optionalString:title]
                     directory:[NativeFileSO optionalString:directory]];
    }
    
    const char* pluginSaveFileSync(const char *name,
                             const char *extension,
                             const char *title,
                             const char *directory) {
        
        return [NativeFileSO fileSaveSync:[NativeFileSO optionalString:extension]
                                     name:[NativeFileSO optionalString:name]
                                    title:[NativeFileSO optionalString:title]
                                directory:[NativeFileSO optionalString:directory]];
    }
    
    void pluginFreeMemory() {
        [NativeFileSO freeDynamicMemory];
    }
}
