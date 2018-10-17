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

import java.io.ByteArrayOutputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;

public class NativeFileOpenActivity extends Activity {

    private final int REQUEST_CODE = 1;

    public static String fileContents = "";

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        String extension = getIntent().getStringExtra("extension");

        Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT); // 4.4+
        //intent.setType("text/" + extension);
        intent.setType("text/plain");
        intent.addCategory(Intent.CATEGORY_OPENABLE);

        startActivityForResult(Intent.createChooser(intent, "Select a file"), REQUEST_CODE);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == REQUEST_CODE && resultCode == Activity.RESULT_OK) {

            if (data != null) {
                Uri uri = data.getData();

                ContentResolver resolver = getContentResolver();
                InputStream stream;
                try {
                    stream = resolver.openInputStream(uri);

                    ByteArrayOutputStream result = new ByteArrayOutputStream();
                    byte[] buffer = new byte[1024];
                    int length;
                    while ((length = stream.read(buffer)) != -1) {
                        Log.d("Buffer write", "B");
                        result.write(buffer, 0, length);
                    }
                    fileContents = result.toString("UTF-8");

                } catch (FileNotFoundException e) {
                    e.printStackTrace();
                    finish();
                    return;
                } catch (IOException e) {
                    e.printStackTrace();
                    finish();
                    return;
                }

                Log.d("Stream Contents", fileContents);

                finish();
            }
        }
    }

    @Override
    protected void onStop() {
        super.onStop();

        UnityPlayer.UnitySendMessage("NativeFileSOMobileCallback",
                "AndroidDidOpenTextFile", fileContents);

        Log.d("SendMessage", "UnitySendMessage " + fileContents);
    }
}
