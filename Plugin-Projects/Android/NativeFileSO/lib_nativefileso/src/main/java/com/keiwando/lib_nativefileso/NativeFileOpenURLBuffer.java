// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

package com.keiwando.lib_nativefileso;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.os.Debug;
import android.provider.OpenableColumns;
import android.util.Log;

import com.keiwando.lib_nativefileso.androidx.annotation.NonNull;

import java.io.*;
import java.util.ArrayList;

public final class NativeFileOpenURLBuffer {

    private static final NativeFileOpenURLBuffer instance = new NativeFileOpenURLBuffer();

    private static final String NATIVE_SO_DIR = "NativeFileSO";

    private final ArrayList<OpenedFile> openedFiles = new ArrayList<>();

    private NativeFileOpenURLBuffer(){}

    public static NativeFileOpenURLBuffer getInstance() {
        return instance;
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

        String result = null;
        if (uri == null || resolver == null) {
            return "";
        }

        try (Cursor cursor = resolver.query(uri, null, null, null, null, null);) {
            if (cursor != null && cursor.moveToFirst()) {
                result = cursor.getString(cursor.getColumnIndexOrThrow(OpenableColumns.DISPLAY_NAME));
            }
        } catch (Exception e) {
            e.printStackTrace();
            Log.d("Plugin DEBUG","Could not retrieve filename");
        }

        if (result == null) {
            result = uri.getPath();
            int cut = result.lastIndexOf('/');
            if (cut != -1) {
                result = result.substring(cut + 1);
            }
        }
        Log.d("Plugin Debug", "File Path: " + uri.toString());
        Log.d("Plugin Debug", "Filename: " + result);
        return result;
    }

    private long getFileSizeFromUri(Uri uri, ContentResolver resolver) {

        long size = -1;
        if (uri == null || resolver == null) {
            return size;
        }

        try (Cursor cursor = resolver.query(uri, null, null, null, null, null)) {
            if (cursor != null && cursor.moveToFirst()) {
                size = cursor.getLong(cursor.getColumnIndexOrThrow(OpenableColumns.SIZE));
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

        File[] files = getNativeSODir(cacheDir).listFiles();
        if (files == null) {
            return;
        }
        for (File file : files) {

            String filename = getFilenameFromUri(Uri.fromFile(file), resolver);
            // long size = getFileSizeFromUri(Uri.fromFile(file), resolver);
            OpenedFile openedFile = new OpenedFile(filename, data, file.getPath());
            openedFiles.add(openedFile);
            Log.d("Plugin DEBUG", "Loaded file from Cache Dir");
            //file.delete();
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
