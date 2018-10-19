package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.os.Debug;
import android.util.Log;
import com.keiwando.lib_nativefileso.androidx.annotation.Nullable;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import java.io.*;

public class NativeFileOpenActivity extends Activity {

    private final int REQUEST_CODE = 1;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent lastIntent = getIntent();

        if (lastIntent.getBooleanExtra("openedFromNativeFileSO", false)) {

            String mimetype = lastIntent.getStringExtra("mimetype");

            Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT); // 4.4+
            //intent.setType("text/" + extension);
            intent.setType(mimetype);
            intent.addCategory(Intent.CATEGORY_OPENABLE);

            Log.d("Plugin DEBUG", "Showing Chooser");

            startActivityForResult(Intent.createChooser(intent, "Select a file"), REQUEST_CODE);

        } else {

            Log.d("Plugin DEBUG", "Opened externally");

            // File is trying to be openend externally
            Uri uri = (Uri)lastIntent.getExtras().get(Intent.EXTRA_STREAM);
            if (uri != null) {

                //NativeFileOpenURLBuffer.getInstance().loadFileFromUri(uri, getContentResolver());
                NativeFileOpenURLBuffer.getInstance().saveFileInCacheDir(uri, getCacheDir(), getContentResolver());

                Intent launchIntent = getPackageManager().getLaunchIntentForPackage(UnityPlayer.currentActivity.getPackageName());
                launchIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK);
                launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(launchIntent);

            } else {
                Log.d("Plugin DEBUG", "No Uri");
                finish();
            }
        }


    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        Log.d("Plugin DEBUG", "OnActivity result " + (data == null));

        if (requestCode == REQUEST_CODE && resultCode == Activity.RESULT_OK) {

            if (data != null) {
                Uri uri = data.getData();

                NativeFileOpenURLBuffer.getInstance().loadFileFromUri(uri, getContentResolver());
            }
        }

        finish();
    }

    @Override
    protected void onStop() {
        super.onStop();

        //UnityPlayer.UnitySendMessage("NativeFileSOMobileCallback",
        //        "AndroidDidOpenTextFile", "");

        //Log.d("SendMessage", "UnitySendMessage ");
    }
}
