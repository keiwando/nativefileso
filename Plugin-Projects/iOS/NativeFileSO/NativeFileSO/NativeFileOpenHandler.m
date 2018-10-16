//
//  NativeFileOpenHandler.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 16.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import "NativeFileOpenHandler.h"
#import "NativeFileOpenURLBuffer.h"
#import "EPPZSwizzler/EPPZSwizzler.h"

@implementation NativeFileOpenHandler

+(void)load {
    NSLog(@"NativeFileOpenHandler load");
    [self swizzle];
}

+(void)swizzle {
    
    [self replaceAppDelegateMethod:@selector(application:openURL:options:)
                         fromClass:NativeFileOpenHandler.class
                  savingOriginalTo:@selector(native_file_open_copy_application:openURL:options:)];
    
    [self replaceAppDelegateMethod:@selector(application:openURL:sourceApplication:annotation:)
                         fromClass:NativeFileOpenHandler.class
                  savingOriginalTo:@selector(native_file_open_copy_application:openURL:sourceApplication:annotation:)];
}

// From: https://github.com/eppz/Unity.Blog.Override_App_Delegate/blob/3.2/iOS/Override_iOS/Override_iOS/Override_iOS.m
+(void)replaceAppDelegateMethod:(SEL) unitySelector
                      fromClass:(Class) overrideAppDelegate
               savingOriginalTo:(SEL) savingOriginalSelector
{
    // The Unity base app controller class (the class name stored in `AppControllerClassName).
    Class unityAppDelegate = NSClassFromString(@"UnityAppController");
    
    // See log messages for the sake of this tutorial.
    [EPPZSwizzler setLogging:YES];
    
    // Add empty placholder to Unity app delegate.
    [EPPZSwizzler addInstanceMethod:savingOriginalSelector
                            toClass:unityAppDelegate
                          fromClass:overrideAppDelegate];
    
    // Save the original Unity app delegate implementation into.
    [EPPZSwizzler swapInstanceMethod:savingOriginalSelector
                  withInstanceMethod:unitySelector
                             ofClass:unityAppDelegate];
    
    // Replace Unity app delegate with ours.
    [EPPZSwizzler replaceInstanceMethod:unitySelector
                                ofClass:unityAppDelegate
                              fromClass:overrideAppDelegate];
}

// MARK: - Appdelegate functions

- (BOOL)application:(UIApplication *)app
            openURL:(NSURL *)url
            options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options {
    
    NSLog(@"Override openURL");
    [[NativeFileOpenURLBuffer instance] loadBufferFromURL:url];
    
    return [self native_file_open_copy_application:app openURL:url options:options];
}

- (BOOL)native_file_open_copy_application:(UIApplication *)app
                                  openURL:(NSURL *)url
                                  options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options {
    
    return YES;
}

-(BOOL)application:(UIApplication*) application
           openURL:(NSURL*) url
 sourceApplication:(NSString*) sourceApplication
        annotation:(id) annotation
{
    NSLog(@"[OverrideAppDelegate application:%@ openURL:%@ sourceApplication:%@ annotation:%@]", application, url, sourceApplication, annotation);
    
    [[NativeFileOpenURLBuffer instance] loadBufferFromURL:url];
    
    return [self native_file_open_copy_application:application
                                                 openURL:url
                                       sourceApplication:sourceApplication
                                              annotation:(annotation) ? annotation : [NSDictionary new]];
}

-(BOOL)native_file_open_copy_application:(UIApplication*) application
                                       openURL:(NSURL*) url
                             sourceApplication:(NSString*) sourceApplication
                                    annotation:(id) annotation
{ return YES; }

@end
