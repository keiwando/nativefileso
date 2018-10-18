package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import com.keiwando.lib_nativefileso.androidx.core.content.FileProvider;
import com.unity3d.player.UnityPlayer;

import java.io.File;

public class NativeFileSO {

    private final static String AUTHORITY = "com.keiwando.nativefileso.provider";

    private final static NativeFileOpenURLBuffer fileBuffer = NativeFileOpenURLBuffer.getInstance();

    public static String GetFileTextContents() {
        return fileBuffer.getTextContents();
    }

    public static byte[] GetFileByteContents() {
        return fileBuffer.getByteContents();
    }

    public static boolean IsTextFile() {
        return fileBuffer.isTextFile();
    }

    public static boolean IsFileLoaded() {
        return fileBuffer.isFileLoaded();
    }

    public static String GetFileName() {
        return fileBuffer.getFilename();
    }

    public static String GetFileExtension() {
        return fileBuffer.getFileExtension();
    }

    public static void ResetLoadedFile() {
        fileBuffer.reset();
    }

    public static boolean IsTemporaryFileAvailable() {

        Context context = UnityPlayer.currentActivity;
        return fileBuffer.isTemporaryFileAvailable(context.getCacheDir());
    }

    public static void LoadTemporaryFile() {
        Context context = UnityPlayer.currentActivity;
        fileBuffer.loadFromTempDir(context.getCacheDir(), context.getContentResolver());
    }

    public static void OpenFile(Activity context, String extension) {

        Intent intent = new Intent(context, NativeFileOpenActivity.class);
        intent.putExtra("extension", extension);
        intent.putExtra("openedFromNativeFileSO", true);

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