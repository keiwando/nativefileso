package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.StrictMode;
import android.os.StrictMode.VmPolicy;
//import androidx.core.content.FileProvider;
import com.keiwando.lib_nativefileso.androidx.core.content.FileProvider;

import java.io.File;
import java.util.List;

public class NativeFileSO {

    private final static String AUTHORITY = "com.keiwando.nativefileso.provider";

    public static void SaveFile(Context context, String srcPath) {

        File file = new File(srcPath);
        Uri contentURI = FileProvider.getUriForFile(context, AUTHORITY, file);

        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        shareIntent.putExtra(Intent.EXTRA_STREAM, contentURI);

        // TODO: Set propert file type
        shareIntent.setType("text/plain");

        context.startActivity(Intent.createChooser(shareIntent, "Share"));
    }
}