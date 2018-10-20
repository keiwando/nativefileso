package com.keiwando.lib_nativefileso;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.provider.OpenableColumns;
import android.util.Log;

import java.io.*;

public class NativeFileOpenURLBuffer {

    private static final NativeFileOpenURLBuffer instance = new NativeFileOpenURLBuffer();

    private static final String NATIVE_SO_DIR = "NativeFileSO";
    private static final String TEMP_FILE_INFO = "TempFileMeta.txt";

    private String filename = "";
    private byte[] contents = new byte[0];

    private boolean isFileLoaded = false;


    private NativeFileOpenURLBuffer(){}

    public static NativeFileOpenURLBuffer getInstance() {
        return instance;
    }

    public void loadFileFromUri(Uri uri, ContentResolver resolver) {

        Log.d("Plugin DEBUG", "Start loading file");

        InputStream stream;
        try {
            stream = resolver.openInputStream(uri);

            if (stream == null) {
                reset();
                return;
            }

            ByteArrayOutputStream result = new ByteArrayOutputStream();
            byte[] buffer = new byte[1024];
            int length;
            while ((length = stream.read(buffer)) != -1) {
                result.write(buffer, 0, length);
            }

            contents = result.toByteArray();

        } catch (FileNotFoundException e) {
            e.printStackTrace();
            return;
        } catch (IOException e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG", "EXCEPTION: File did not finish loading!");
            return;
        }

        saveName(uri, resolver);

        Log.d("Plugin DEBUG", "File is Loaded in Plugin!");
        isFileLoaded = true;
    }

    private void saveName(Uri uri, ContentResolver resolver) {

        Cursor cursor = resolver.query(uri, null, null, null, null, null);

        try {
            if (cursor != null && cursor.moveToFirst()) {
                filename = cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME));
                return;
            }
        } catch (Exception e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG","Could not retrieve filename");
        } finally {
            if (cursor != null) {
                cursor.close();
            }
        }

        // Something went wrong
        filename = "";
    }

    public void saveFileInCacheDir(Uri uri, File cacheDir, ContentResolver resolver) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        loadFileFromUri(uri, resolver);
        File tempFile = new File(nativeSODir, filename);

        File metafile = new File(nativeSODir, TEMP_FILE_INFO);

        try {

            FileOutputStream out = new FileOutputStream(tempFile);
            out.write(contents);
            out.close();

            out = new FileOutputStream(metafile);
            out.write(filename.getBytes());
            out.close();

            Log.d("Plugin DEBUG", "Saved in Cache Dir");

        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void loadFromTempDir(File cacheDir, ContentResolver resolver) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        File metafile = new File(nativeSODir, TEMP_FILE_INFO);

        try {
            FileInputStream in = new FileInputStream(metafile);
            int length = (int)metafile.length();
            byte[] bytes = new byte[length];
            in.read(bytes);
            in.close();

            String filename = new String(bytes);
            File tempFile = new File(nativeSODir, filename);

            loadFileFromUri(Uri.fromFile(tempFile), resolver);

            this.filename = filename;

            tempFile.delete();

            Log.d("Plugin DEBUG", "Loaded from Cache Dir");

        } catch (IOException e) {
            e.printStackTrace();
            reset();
            return;
        }
    }

    public boolean isTemporaryFileAvailable(File cacheDir) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        File metafile = new File(nativeSODir, TEMP_FILE_INFO);

        try {
            FileInputStream in = new FileInputStream(metafile);
            int length = (int)metafile.length();
            byte[] bytes = new byte[length];
            in.read(bytes);
            in.close();

            String filename = new String(bytes);
            File tempFile = new File(nativeSODir, filename);

            return tempFile.exists() && !tempFile.isDirectory();

        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    public void reset() {

        isFileLoaded = false;
        contents = new byte[0];
        filename = "";
    }

    public byte[] getByteContents() {
        return contents;
    }

    public String getFilename() {
        return filename;
    }

    public boolean isFileLoaded() {
        return isFileLoaded;
    }
}
