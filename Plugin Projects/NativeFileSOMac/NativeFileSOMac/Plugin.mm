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

extern "C" {
    
    const char* _openFile(const char* extensions) {

        return [NativeFileSO fileOpen:[NSString stringWithUTF8String:extensions]];
    }
    
    const char* _saveFile(const char* name,
                          const char* extension) {
        
        return [NativeFileSO fileSave:[NSString stringWithUTF8String:extension]
                                 name:[NSString stringWithUTF8String:name]];
    }
}
