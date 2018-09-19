//
//  NativeFileSO.m
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 19.09.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeFileSO.h"

@interface NativeFileSO () {
    // Class extension for private properties
}

@end

@implementation NativeFileSO

+ (const char *)fileOpen:(NSArray<NSString *> *)fileExtensions {
    
    NSOpenPanel* panel = [self createOpenPanel:fileExtensions];
    NSModalResponse response = [panel runModal];
    
    if (response == NSModalResponseOK) {
        return [panel.URL.path UTF8String];
    } else {
        return "";
    }
}

+ (const char *)fileSave:(NSString *)originalPath {
    
    NSSavePanel* panel = [NSSavePanel savePanel];
    NSModalResponse response = [panel runModal];
    
    if (response == NSModalResponseOK) {
        return [panel.URL.path UTF8String];
    } else {
        return "";
    }
}

+ (NSOpenPanel *)createOpenPanel:(NSArray<NSString *> *)fileExtensions {
    
    NSOpenPanel *panel = [NSOpenPanel openPanel];
    
    [panel setCanChooseDirectories:NO];
    [panel setAllowedFileTypes:fileExtensions];
    [panel setFloatingPanel:YES];
    [panel setCanChooseFiles:TRUE];
    [panel setAllowsMultipleSelection:NO];
    
    return panel;
}

@end
