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
                [paths addObject:URLs[i]];
            }
            _bufferedPaths = [self copyStrings:paths];
            
            if (_bufferedPaths) {
                callback(YES, _bufferedPaths);
            } else {
                callback(NO, nil);
            }
        } else {
            callback(NO, nil);
        }
    }];
}

+ (char **) fileOpenSync:(NSString *)extensions
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
            [paths addObject:URLs[i]];
        }
        _bufferedPaths = [self copyStrings:paths];
        
        if (_bufferedPaths) {
            return _bufferedPaths;
        } else {
            return nil;
        }
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
    //NSModalResponse response = [panel runModal];
    
    NSWindow *window = [[NSApplication sharedApplication] mainWindow];
    
    if (window == nil) { return; }
    
    [panel beginSheetModalForWindow:window completionHandler:^(NSModalResponse response) {
        
        NSLog(@"Sheet modal ended.");
        
        if (callback == nil) {
            NSLog(@"No callback set.");
            return;
        }
        
        if (response == NSModalResponseOK) {
            NSArray *paths = @[panel.URL.path];
            _bufferedPaths = [self copyStrings:paths];
            
            char *testPaths[] = { "~/Desktop/TestPath.evol" };
            
            if (_bufferedPaths) {
                callback(YES, testPaths);
                //callback(YES, _bufferedPaths);
            } else {
                callback(NO, nil);
            }
        } else {
            NSLog(@"File Save Canceled!");
            callback(NO, nil);
        }
    }];
}

+ (char *) fileSaveSync:(NSString *)extension
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
        NSArray *paths = @[panel.URL.path];
        _bufferedPaths = [self copyStrings:paths];
        
        if (_bufferedPaths) {
            return _bufferedPaths[0];
        } else {
            return (char *)[@"" UTF8String];
        }
    } else {
        return (char *)[@"" UTF8String];
    }
}

+ (NSOpenPanel *)createOpenPanel:(NSArray<NSString *> *)fileExtensions
               canSelectMultiple:(bool)canSelectMultiple
                           title:(NSString *)title
                       directory:(NSString *)directory {
    
    NSOpenPanel *panel = [NSOpenPanel openPanel];
    
    [panel setCanChooseDirectories:NO];
    [panel setAllowedFileTypes:fileExtensions];
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
    
    return panel;
}

+ (NSSavePanel *)createSavePanel:(NSArray<NSString *> *)fileExtensions
                            name:(NSString *)name
                           title:(NSString *)title
                       directory:(NSString *)directory {
    
    NSSavePanel *panel = [NSSavePanel savePanel];
    
    [panel setCanCreateDirectories:YES];
    [panel setAllowedFileTypes:fileExtensions];
    [panel setFloatingPanel:NO];
    [panel setCanCreateDirectories:YES];
    [panel setNameFieldStringValue:name];
    [panel setExtensionHidden:NO];
    [panel setCanSelectHiddenExtension:YES];
    
    if (title != nil) {
        [panel setTitle:title];
    }
    
    if (directory != nil) {
        NSURL *directoryURL = [NSURL URLWithString:directory];
        if (directoryURL != nil) {
            [panel setDirectoryURL:directoryURL];
        }
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
            char *copy = (char *)malloc([str lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1);
            
            if (!copy) {
                NSLog(@"Could not allocate enough memory for the string copy at index %lu", (unsigned long)i);
                copies[i] = (char*)[@"" UTF8String];
            } else {
                copies[i] = copy;
            }
        }
    }
    
    return copies;
}

+ (void) freeDynamicMemory {
    
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

@end
