//
//  Plugin.m
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 18.09.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

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
    
    char** pluginOpenFileSync(const char *extensions,
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
    
    char* pluginSaveFileSync(const char *name,
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
