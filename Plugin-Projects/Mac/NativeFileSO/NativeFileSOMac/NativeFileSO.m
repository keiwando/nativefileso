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

static UnityCallbackFunction callback;
static char** _bufferedPaths;

+ (void)setCallback:(UnityCallbackFunction)unityCallback {
    callback = unityCallback;
}

+ (void) fileOpen:(NSString *)extensions
canSelectMultiple:(bool)canSelectMultiple
            title:(NSString *)title
        directory:(NSString *)directory {
    
    NSArray *fileExtensions = [self extractExtensions:extensions];
    NSOpenPanel* panel = [self createOpenPanel:fileExtensions
                             canSelectMultiple:canSelectMultiple
                                         title:title
                                     directory:directory];
    
    NSWindow *window = [[NSApplication sharedApplication] mainWindow];
    
    if (window == nil) { return; }
    
    [panel beginSheetModalForWindow:window completionHandler:^(NSModalResponse response){
        if (callback == nil) return;
        
        if (response == NSModalResponseOK) {
            NSArray *URLs = panel.URLs;
            NSMutableArray *paths = [NSMutableArray arrayWithCapacity:URLs.count];
            for (int i = 0; i < (int)URLs.count; i++) {
                [paths addObject:[URLs[i] path]];
            }
            NSString *pathsString = [paths componentsJoinedByString:[NSString stringWithFormat:@"%c", 28]];
            
            [self sendCallback:YES paths:[pathsString UTF8String]];
        } else {
            [self sendCallback:NO paths:nil];
        }
        [self freeDynamicMemory];
    }];
}

+ (const char *) fileOpenSync:(NSString *)extensions
      canSelectMultiple:(bool)canSelectMultiple
                  title:(NSString *)title
              directory:(NSString *)directory {
    
    NSArray *fileExtensions = [self extractExtensions:extensions];
    NSOpenPanel* panel = [self createOpenPanel:fileExtensions
                             canSelectMultiple:canSelectMultiple
                                         title:title
                                     directory:directory];
    
    NSModalResponse response = [panel runModal];
    
    if (response == NSModalResponseOK) {
        NSArray *URLs = panel.URLs;
        NSMutableArray *paths = [NSMutableArray arrayWithCapacity:URLs.count];
        for (int i = 0; i < (int)URLs.count; i++) {
            [paths addObject:[URLs[i] path]];
        }
        NSString *pathsString = [paths componentsJoinedByString:[NSString stringWithFormat:@"%c", 28]];
        
        return [pathsString UTF8String];
    } else {
        return nil;
    }
}

+ (void)fileSave:(NSString *)extension
            name:(NSString *)name
           title:(NSString *)title
       directory:(NSString *)directory {
    
    NSArray *fileExtensions = [NSArray arrayWithObjects:extension, nil];
    NSSavePanel* panel = [self createSavePanel:fileExtensions
                                          name:name
                                         title:title
                                     directory:directory];
    
    NSWindow *window = [[NSApplication sharedApplication] mainWindow];
    
    if (window == nil) { return; }
    
    [panel beginSheetModalForWindow:window completionHandler:^(NSModalResponse response) {
        
        if (callback == nil) {
            return;
        }
        
        if (response == NSModalResponseOK) {
            
            [self sendCallback:YES paths:[panel.URL.path UTF8String]];
        } else {
            [self sendCallback:NO paths:nil];
        }
        [self freeDynamicMemory];
    }];
}

+ (const char *) fileSaveSync:(NSString *)extension
                         name:(NSString *)name
                        title:(NSString *)title
                    directory:(NSString *)directory {
    
    NSArray *fileExtensions = [NSArray arrayWithObjects:extension, nil];
    NSSavePanel* panel = [self createSavePanel:fileExtensions
                                          name:name
                                         title:title
                                     directory:directory];
    
    NSModalResponse response = [panel runModal];
    
    if (response == NSModalResponseOK) {
        return [panel.URL.path UTF8String];
    } else {
        return nil;
    }
}

+ (NSOpenPanel *)createOpenPanel:(NSArray<NSString *> *)fileExtensions
               canSelectMultiple:(bool)canSelectMultiple
                           title:(NSString *)title
                       directory:(NSString *)directory {
    
    NSOpenPanel *panel = [NSOpenPanel openPanel];
    
    [panel setCanChooseDirectories:NO];
    [panel setFloatingPanel:NO];
    [panel setCanChooseFiles:TRUE];
    [panel setCanCreateDirectories:YES];
    [panel setAllowsMultipleSelection:canSelectMultiple];
    
    if (title != nil) {
        [panel setTitle:title];
    }
    
    if (directory != nil) {
        NSURL *directoryURL = [NSURL URLWithString:directory];
        if (directoryURL != nil) {
            [panel setDirectoryURL:directoryURL];
        }
    }
    
    if (fileExtensions.count == 0 || [fileExtensions containsObject:@"*"]) {
        [panel setAllowedFileTypes:nil];
    } else {
        [panel setAllowedFileTypes:fileExtensions];
    }
    
    return panel;
}

+ (NSSavePanel *)createSavePanel:(NSArray<NSString *> *)fileExtensions
                            name:(NSString *)name
                           title:(NSString *)title
                       directory:(NSString *)directory {
    
    NSSavePanel *panel = [NSSavePanel savePanel];
    
    [panel setCanCreateDirectories:YES];
    [panel setFloatingPanel:NO];
    [panel setCanCreateDirectories:YES];
    [panel setNameFieldStringValue:name];
    [panel setExtensionHidden:NO];
    [panel setCanSelectHiddenExtension:NO];
    
    if (title != nil) {
        [panel setTitle:title];
    }
    
    if (directory != nil) {
        NSURL *directoryURL = [NSURL URLWithString:directory];
        if (directoryURL != nil) {
            [panel setDirectoryURL:directoryURL];
        }
    }
    
    if (fileExtensions.count == 0 || [fileExtensions containsObject:@"*"]) {
        [panel setAllowedFileTypes:nil];
    } else {
        [panel setAllowedFileTypes:fileExtensions];
    }
    
    return panel;
}

+ (NSArray<NSString *> *)extractExtensions:(NSString *)extensions {
    
    return [extensions componentsSeparatedByString:@"%"];
}

+ (char **) copyStrings:(NSArray<NSString *> *)strings {
    
    char **copies = nil;
    NSUInteger strCount = strings.count;
    
    if (strCount > 0) {
        copies = (char **)calloc(strCount, sizeof(char *));
        
        if (!copies) {
            NSLog(@"Could not allocate enough memory.");
            return nil;
        }
        
        for (NSUInteger i = 0; i < strCount; i++) {
            NSString *str = strings[i];
            const size_t len = [str lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1;
            char *copy = (char *)malloc(len);
            
            if (!copy) {
                NSLog(@"Could not allocate enough memory for the string copy at index %lu", (unsigned long)i);
                copies[i] = (char*)[@"" UTF8String];
            } else {
                strncpy(copy, [str UTF8String], len);
                copies[i] = copy;
            }
        }
    }
    
    return copies;
}

+ (void) freeDynamicMemory {
    
    if (_bufferedPaths == nil) { return; }
    
    int count = (int)( sizeof(_bufferedPaths) / sizeof(char *));
    for (int i = 0; i < count; i++) {
        if (_bufferedPaths[i]) {
            free(_bufferedPaths[i]);
        }
    }
    free(_bufferedPaths);
    _bufferedPaths = nil;
}

+ (NSString *) optionalString:(const char *)cString {
    return cString == nil ? nil : [NSString stringWithUTF8String:cString];
}

+ (void) sendCallback:(bool)pathsSelected
                paths:(const char *)paths {
    
    if (callback != nil) {
        dispatch_async(dispatch_get_main_queue(), ^(void){
            callback(pathsSelected, paths);
        });
    }
}


@end
