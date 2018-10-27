package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import com.keiwando.lib_nativefileso.androidx.core.content.FileProvider;

import java.io.File;

public class NativeFileSO {

    private final static String AUTHORITY = "com.keiwando.nativefileso.{applicationId}.provider";

    private final static NativeFileOpenURLBuffer fileBuffer = NativeFileOpenURLBuffer.getInstance();

    public static byte[] GetFileByteContents() {
        return fileBuffer.getByteContents();
    }

    public static boolean IsFileLoaded() {
        return fileBuffer.isFileLoaded();
    }

    public static String GetFileName() {
        return fileBuffer.getFilename();
    }

    public static void ResetLoadedFile() {
        fileBuffer.reset();
    }

    public static boolean IsTemporaryFileAvailable(Activity context) {

        return fileBuffer.isTemporaryFileAvailable(context.getCacheDir());
    }

    public static void LoadTemporaryFile(Activity context) {

        fileBuffer.loadFromTempDir(context.getCacheDir(), context.getContentResolver());
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

        context.startActivity(Intent.createChooser(shareIntent, "Share"));
    }
}