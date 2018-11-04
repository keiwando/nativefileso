package com.keiwando.lib_nativefileso;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.os.Debug;
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

        InputStream stream = null;
        try {
            stream = resolver.openInputStream(uri);

            if (stream == null) {
                return null;
            }

            int size = (int)Math.min(Integer.MAX_VALUE, getFileSizeFromUri(uri, resolver));
            Log.d("Plugin Debug", "File size: " + size);

            ByteArrayOutputStream result;
            byte[] data;

            if (size == -1) {
                result = new ByteArrayOutputStream();
                byte[] buffer = new byte[1024];
                int length;
                while ((length = stream.read(buffer)) != -1) {
                    result.write(buffer, 0, length);
                }
                data = result.toByteArray();

            } else {

                data = new byte[size];
                stream.read(data, 0, data.length);
            }

            String filename = getFilenameFromUri(uri, resolver);

            Log.d("Plugin DEBUG", "A file was loaded in Plugin!");
            stream.close();

            return new OpenedFile(filename, data, uri.getPath());

        } catch (FileNotFoundException e) {
            e.printStackTrace();
            //stream.close();
            return null;
        } catch (IOException e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG", "EXCEPTION: File did not finish loading!");
            return null;
        }
    }

    public void saveFileFromUriToFolder(Uri uri, File file, ContentResolver resolver) {

        Log.d("Plugin DEBUG", "Start loading file");

        InputStream stream = null;
        try {
            stream = resolver.openInputStream(uri);

            if (stream == null) {
                return;
            }

            FileOutputStream output = new FileOutputStream(file);

            byte[] buffer = new byte[10240];
            int length;
            while ((length = stream.read(buffer)) != -1) {
                output.write(buffer, 0, length);
            }

            stream.close();

        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private String getFilenameFromUri(Uri uri, ContentResolver resolver) {

        Cursor cursor = resolver.query(uri, null, null, null, null, null);
        String result = null;

        try {
            if (cursor != null && cursor.moveToFirst()) {
                result = cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME));
            }
        } catch (Exception e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG","Could not retrieve filename");
        } finally {
            if (cursor != null) {
                cursor.close();
            }
        }

        if (result == null) {
            result = uri.getPath();
            int cut = result.lastIndexOf('/');
            if (cut != -1) {
                result = result.substring(cut + 1);
            }
        }
        Log.d("Plugin Debug", "File Path: " + uri.getPath());
        Log.d("Plugin Debug", "Filename: " + result);
        return result;
    }

    private long getFileSizeFromUri(Uri uri, ContentResolver resolver) {

        Cursor cursor = resolver.query(uri, null, null, null, null, null);
        long size = -1;

        try {
            if (cursor != null && cursor.moveToFirst()) {
                size = cursor.getLong(cursor.getColumnIndex(OpenableColumns.SIZE));
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        return size;
    }

    public void saveFilesInCacheDir(ArrayList<Uri> uris, File cacheDir, ContentResolver resolver) {

        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        clearDirectory(nativeSODir);

        for (Uri uri : uris) {

            File tempFile = new File(nativeSODir, getFilenameFromUri(uri, resolver));
            saveFileFromUriToFolder(uri, tempFile, resolver);

            Log.d("Plugin DEBUG", "Saved file in Cache Dir");
        }
    }

    public void loadFromTempDir(File cacheDir, ContentResolver resolver) {

        byte[] data = new byte[0];

        for (File file : getNativeSODir(cacheDir).listFiles()) {

            //OpenedFile openedFile = loadFileFromUri(Uri.fromFile(file), resolver);
            String filename = getFilenameFromUri(Uri.fromFile(file), resolver);
            long size = getFileSizeFromUri(Uri.fromFile(file), resolver);
            OpenedFile openedFile = new OpenedFile(filename, data, file.getPath());
            if (openedFile != null) {
                openedFiles.add(openedFile);
                Log.d("Plugin DEBUG", "Loaded file from Cache Dir");
                //file.delete();
            }
        }
    }

    public void freeMemory(File cacheDir) {
        openedFiles.clear();
        clearDirectory(getNativeSODir(cacheDir));
        Log.d("Plugin DEBUG", "Freed Memory");
    }

    private File getNativeSODir(File cacheDir) {
        File nativeSODir = new File(cacheDir, NATIVE_SO_DIR);
        nativeSODir.mkdirs();
        return nativeSODir;
    }

    /*
    * Non-recursive!
    */
    private void clearDirectory(File directory) {
        File[] files = directory.listFiles();
        if (files != null) {
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
