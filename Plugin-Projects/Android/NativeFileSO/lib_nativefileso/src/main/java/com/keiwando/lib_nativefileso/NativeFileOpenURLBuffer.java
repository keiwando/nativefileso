package com.keiwando.lib_nativefileso;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.provider.OpenableColumns;
import android.util.Log;

import java.io.*;
import java.util.ArrayList;

public class NativeFileOpenURLBuffer {

    private static final NativeFileOpenURLBuffer instance = new NativeFileOpenURLBuffer();

    private static final String NATIVE_SO_DIR = "NativeFileSO";

    private ArrayList<OpenedFile> openedFiles = new ArrayList<>();

    private NativeFileOpenURLBuffer(){}

    public static NativeFileOpenURLBuffer getInstance() {
        return instance;
    }

    public void refreshBufferWithFileFromUri(Uri uri, ContentResolver resolver) {
        openedFiles.clear();
        OpenedFile file = loadFileFromUri(uri, resolver);
        if (file != null) {
            openedFiles.add(file);
        }
    }

    public void refreshBufferWithUris(ArrayList<Uri> uris, ContentResolver resolver) {
        openedFiles.clear();
        for(Uri uri : uris) {
            OpenedFile file = loadFileFromUri(uri, resolver);
            if (file != null) {
                openedFiles.add(file);
            }
        }
    }

    public OpenedFile loadFileFromUri(Uri uri, ContentResolver resolver) {

        Log.d("Plugin DEBUG", "Start loading file");

        InputStream stream;
        try {
            stream = resolver.openInputStream(uri);

            if (stream == null) {
                return null;
            }

            ByteArrayOutputStream result = new ByteArrayOutputStream();
            byte[] buffer = new byte[1024];
            int length;
            while ((length = stream.read(buffer)) != -1) {
                result.write(buffer, 0, length);
            }

            byte[] data = result.toByteArray();
            String filename = getFilenameFromUri(uri, resolver);

            Log.d("Plugin DEBUG", "A file was loaded in Plugin!");

            return new OpenedFile(filename, data);

        } catch (FileNotFoundException e) {
            e.printStackTrace();
            return null;
        } catch (IOException e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG", "EXCEPTION: File did not finish loading!");
            return null;
        }
    }

    private String getFilenameFromUri(Uri uri, ContentResolver resolver) {

        Cursor cursor = resolver.query(uri, null, null, null, null, null);

        try {
            if (cursor != null && cursor.moveToFirst()) {
                return cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME));
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
        return "Unnamed";
    }

    public void saveFilesInCacheDir(ArrayList<Uri> uris, File cacheDir, ContentResolver resolver) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        clearDirectory(nativeSODir);

        for (Uri uri : uris) {

            OpenedFile openedFile = loadFileFromUri(uri, resolver);
            File tempFile = new File(nativeSODir, openedFile.getFilename());

            try {

                FileOutputStream out = new FileOutputStream(tempFile);
                out.write(openedFile.getData());
                out.close();

                Log.d("Plugin DEBUG", "Saved in Cache Dir");

            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    public void loadFromTempDir(File cacheDir, ContentResolver resolver) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();

        for (File file : nativeSODir.listFiles()) {

            OpenedFile openedFile = loadFileFromUri(Uri.fromFile(file), resolver);
            if (openedFile != null) {
                openedFiles.add(openedFile);
                Log.d("Plugin DEBUG", "Loaded file from Cache Dir");
                file.delete();
            }
        }
    }

    public void freeMemory() {
        openedFiles.clear();
    }

    /*
    * Non-recursive!
    */
    private void clearDirectory(File directory) {
        File[] files = directory.listFiles();
        if (files == null) {
            for (File f : files) {
                f.delete();
            }
        }
    }

    public int getNumberOfLoadedFiles() {
        return openedFiles.size();
    }

    public OpenedFile getOpenedFileAtIndex(int index) {
        return openedFiles.get(index);
    }
}
