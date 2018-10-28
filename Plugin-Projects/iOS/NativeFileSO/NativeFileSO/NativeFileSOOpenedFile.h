//
//  NativeFileSOOpenedFile.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 28.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#ifndef NativeFileSOOpenedFile_h
#define NativeFileSOOpenedFile_h

struct NativeFileSOOpenedFile {
    const char *filename;
    const void *data;
    int dataLength;
};

#endif /* NativeFileSOOpenedFile_h */
