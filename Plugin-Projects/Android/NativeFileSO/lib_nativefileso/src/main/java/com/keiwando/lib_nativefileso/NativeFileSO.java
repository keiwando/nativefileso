package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import com.keiwando.lib_nativefileso.androidx.core.content.FileProvider;

import java.io.File;

public class NativeFileSO {

    private final static String AUTHORITY = "com.keiwando.nativefileso.provider";

    public static String GetFileContents() {

        return NativeFileOpenActivity.fileContents;
    }

    public static void OpenFile(Activity context, String extension) {

        Intent intent = new Intent(context, NativeFileOpenActivity.class);
        intent.putExtra("extension", extension);

        context.startActivity(intent);
    }

    public static void SaveFile(Context context, String srcPath) {

        File file = new File(srcPath);
        Uri contentURI = FileProvider.getUriForFile(context, AUTHORITY, file);

        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        shareIntent.putExtra(Intent.EXTRA_STREAM, contentURI);

        // TODO: Set proper file type
        shareIntent.setType("text/plain");

        context.startActivity(Intent.createChooser(shareIntent, "Share"));
    }
}