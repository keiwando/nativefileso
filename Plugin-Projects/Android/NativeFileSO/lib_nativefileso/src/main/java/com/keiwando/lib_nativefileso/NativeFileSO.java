// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import com.keiwando.lib_nativefileso.androidx.core.content.FileProvider;

import java.io.File;
import java.util.List;

public class NativeFileSO {

    private final static String AUTHORITY = "com.keiwando.nativefileso.{applicationId}.fileprovider";

    private final static NativeFileOpenURLBuffer fileBuffer = NativeFileOpenURLBuffer.getInstance();

    public static void LoadTemporaryFiles(Activity context) {

        fileBuffer.loadFromTempDir(context.getCacheDir(), context.getContentResolver());
    }

    public static int GetNumberOfLoadedFiles() {
        return fileBuffer.getNumberOfLoadedFiles();
    }

    public static OpenedFile GetLoadedFileAtIndex(int i) {
        return fileBuffer.getOpenedFileAtIndex(i);
    }

    public static void FreeMemory(Activity context) {
        fileBuffer.freeMemory(context.getCacheDir());
    }

    public static void OpenFile(Activity context, String mimetypes) {

        Intent intent = new Intent(context, NativeFileOpenActivity.class);
        intent.putExtra("mimetypes", mimetypes);
        intent.putExtra("openedFromNativeFileSO", true);

        context.startActivity(intent);
    }

    public static void OpenFiles(Activity context, String mimetypes) {

        Intent intent = new Intent(context, NativeFileOpenActivity.class);
        intent.putExtra("mimetypes", mimetypes);
        intent.putExtra("openedFromNativeFileSO", true);
        intent.putExtra("canOpenMultiple", true);

        context.startActivity(intent);
    }

    public static void SaveFile(Context context, String srcPath, String mimeType) {

        File file = new File(srcPath);
        Uri contentURI = FileProvider.getUriForFile(context,
                AUTHORITY.replace("{applicationId}", context.getPackageName()),
                file);

        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        shareIntent.putExtra(Intent.EXTRA_STREAM, contentURI);
        shareIntent.setType(mimeType);

        Intent chooser = Intent.createChooser(shareIntent, "Share");

        List<ResolveInfo> resInfoList = context.getPackageManager().queryIntentActivities(chooser, PackageManager.MATCH_ALL);
        for (ResolveInfo resolveInfo : resInfoList) {
            String packageName = resolveInfo.activityInfo.packageName;
            context.grantUriPermission(packageName, contentURI, Intent.FLAG_GRANT_WRITE_URI_PERMISSION | Intent.FLAG_GRANT_READ_URI_PERMISSION);
        }
        context.startActivity(chooser);
    }
}