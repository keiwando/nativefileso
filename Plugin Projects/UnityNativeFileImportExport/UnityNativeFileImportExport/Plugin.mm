//
//  Plugin.m
//  UnityNativeFileImportExport
//
//  Created by Keiwan Donyagard on 18.09.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

extern "C" {
    
    void test() {
        NSOpenPanel* panel = [NSOpenPanel openPanel];
        NSModalResponse response = [panel runModal];
    }
    
    const char* _openFile() {

        NSOpenPanel* panel = [NSOpenPanel openPanel];
        NSModalResponse response = [panel runModal];

        if (response == NSModalResponseOK) {
            return [panel.URL.path UTF8String];
        } else {
            return "";
        }
    }
    
    const char* _saveFile() {
        
        NSSavePanel* panel = [NSSavePanel savePanel];
        NSModalResponse response = [panel runModal];
        
        if (response == NSModalResponseOK) {
            return [panel.URL.path UTF8String];
        } else {
            return "";
        }
    }
}
