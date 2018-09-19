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
    
    const char* _openFile() {

        NSArray<NSString *> *fileExtensions = @[@"evol", @"creat"];
        return [NativeFileSO fileOpen:fileExtensions];
    }
    
    const char* _saveFile(NSString * srcPath) {
        
        return [NativeFileSO fileSave:srcPath];
    }
}
