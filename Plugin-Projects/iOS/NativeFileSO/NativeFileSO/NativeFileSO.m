//
//  NativeFileSO.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 14.10.18.
//  Copyright © 2018 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#import "NativeFileSO.h"

@implementation NativeFileSO

+ (void)fileOpen:(NSString *)UTIs
  allowsMultiple:(bool)allowsMultiple {
    
    NSArray *utiStrings = [self decodeUTIs:UTIs];
    UIDocumentPickerViewController *documentPicker = [[UIDocumentPickerViewController alloc] initWithDocumentTypes:utiStrings inMode:UIDocumentPickerModeImport];
    
    documentPicker.delegate = [NativeFileOpenURLBuffer instance];
    
    if (@available(iOS 11.0, *)) {
        documentPicker.allowsMultipleSelection = allowsMultiple;
    }
    
    UIViewController *topVC = [self topViewController];
    [topVC presentViewController:documentPicker animated:YES completion:^{}];
}

+ (void)fileSave:(NSString *)srcPath
            name:(NSString *)name {
    
    NSURL *url = [NSURL fileURLWithPath:srcPath];
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc]
                                                        initWithActivityItems:@[url]
                                                        applicationActivities:nil];
    
    [activityViewController setCompletionWithItemsHandler:^(NSString *activityType, BOOL completed, NSArray *returnedItems, NSError *error){
        [[NativeFileOpenURLBuffer instance] sendCallback];
    }];
    
    UIViewController *topVC = [self topViewController];
    
    if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPhone) {
        
        UIView *view = topVC.view;
        CGRect rect = CGRectMake(view.frame.size.width / 2, view.frame.size.height / 2, 1, 1);
        
        activityViewController.popoverPresentationController.sourceView = view;
        activityViewController.popoverPresentationController.sourceRect = rect;
        activityViewController.popoverPresentationController.permittedArrowDirections = 0;
    }
    
    [topVC presentViewController:activityViewController animated:YES completion:^{}];
}



//+ (NSArray<NSString *> *)extractExtensions:(NSString *)extensions {
//
//    return [extensions componentsSeparatedByString:@"%"];
//}

+ (UIViewController *)topViewController{
    return [self topViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

+ (UIViewController *)topViewController:(UIViewController *)rootViewController
{
    if (rootViewController.presentedViewController == nil) {
        return rootViewController;
    }
    
    if ([rootViewController.presentedViewController isKindOfClass:[UINavigationController class]]) {
        UINavigationController *navigationController = (UINavigationController *)rootViewController.presentedViewController;
        UIViewController *lastViewController = [[navigationController viewControllers] lastObject];
        return [self topViewController:lastViewController];
    }
    
    UIViewController *presentedViewController = (UIViewController *)rootViewController.presentedViewController;
    return [self topViewController:presentedViewController];
}

+ (NSArray<NSString *> *)decodeUTIs:(NSString *)UTIs
{
    return [UTIs componentsSeparatedByString:@"|"];
}

@end
