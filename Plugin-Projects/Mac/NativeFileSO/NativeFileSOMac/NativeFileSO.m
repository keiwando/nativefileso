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

+ (const char *)fileOpen:(NSString *)extensions {
    
    NSArray *fileExtensions = [self extractExtensions:extensions];
    NSOpenPanel* panel = [self createOpenPanel:fileExtensions];
    NSModalResponse response = [panel runModal];
    
    if (response == NSModalResponseOK) {
        return [panel.URL.path UTF8String];
    } else {
        return "";
    }
}

+ (const char *)fileSave:(NSString *)extension
                    name:(NSString *)name {
    
    NSArray *fileExtensions = [NSArray arrayWithObjects:extension, nil];
    NSSavePanel* panel = [self createSavePanel:fileExtensions name:name];
    //NSModalResponse response = [panel runModal];
    
    NSWindow *window = [[NSApplication sharedApplication] mainWindow];
    
    if (window == nil) { return ""; }
    
    [panel beginSheetModalForWindow:window completionHandler:^(NSModalResponse response) {
        if (response == NSModalResponseOK) {
            [panel.URL.path UTF8String];
        } else {
            "";
        }
    }];
    
    return "";
}

+ (NSOpenPanel *)createOpenPanel:(NSArray<NSString *> *)fileExtensions {
    
    NSOpenPanel *panel = [NSOpenPanel openPanel];
    
    [panel setCanChooseDirectories:NO];
    [panel setAllowedFileTypes:fileExtensions];
    [panel setFloatingPanel:YES];
    [panel setCanChooseFiles:TRUE];
    [panel setCanCreateDirectories:NO];
    [panel setAllowsMultipleSelection:NO];
    
    return panel;
}

+ (NSSavePanel *)createSavePanel:(NSArray<NSString *> *)fileExtensions
                            name:(NSString *)name {
    
    NSSavePanel *panel = [NSSavePanel savePanel];
    
    [panel setCanCreateDirectories:NO];
    [panel setAllowedFileTypes:fileExtensions];
    [panel setFloatingPanel:YES];
    [panel setCanCreateDirectories:YES];
    [panel setNameFieldStringValue:name];
    
    return panel;
}

+ (NSArray<NSString *> *)extractExtensions:(NSString *)extensions {
    
    return [extensions componentsSeparatedByString:@"%"];
}

@end
