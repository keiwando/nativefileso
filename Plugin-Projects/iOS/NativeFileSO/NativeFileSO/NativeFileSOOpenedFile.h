//
//  NativeFileSOOpenedFile.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 28.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#ifndef NativeFileSOOpenedFile_h
#define NativeFileSOOpenedFile_h

@interface NativeFileSOOpenedFile : NSObject

@property (readonly, nonatomic, strong) NSString *filename;
@property (readonly, nonatomic, strong) NSData *data;

-(id)initWithFilename:(NSString *)filename
                 data:(NSData *)data;

@end

#endif /* NativeFileSOOpenedFile_h */
