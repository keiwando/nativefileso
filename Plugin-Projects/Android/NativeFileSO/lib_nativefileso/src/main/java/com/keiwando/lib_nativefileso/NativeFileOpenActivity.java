package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import com.keiwando.lib_nativefileso.androidx.annotation.Nullable;

public class NativeFileOpenActivity extends Activity {

    private final int REQUEST_CODE = 1;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent lastIntent = getIntent();

        if (lastIntent.getBooleanExtra("openedFromNativeFileSO", false)) {

            Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT); // 4.4+

            String encodedTypes = lastIntent.getStringExtra("mimetypes");
            intent.setType("*/*");

            if (encodedTypes != null && !encodedTypes.equals("")) {
                String[] mimeTypes = encodedTypes.split(" ");

                if (mimeTypes.length == 1) {
                    intent.setType(mimeTypes[0]);
                } else {
                    intent.putExtra(Intent.EXTRA_MIME_TYPES, mimeTypes);
                }
            }

            intent.addCategory(Intent.CATEGORY_OPENABLE);

            Log.d("Plugin DEBUG", "Showing Chooser");

            startActivityForResult(Intent.createChooser(intent, "Select a file"), REQUEST_CODE);

        } else {

            Log.d("Plugin DEBUG", "Opened externally");

            // File is trying to be openend externally
            Uri uri = (Uri)lastIntent.getExtras().get(Intent.EXTRA_STREAM);
            if (uri != null) {

                NativeFileOpenURLBuffer.getInstance().saveFileInCacheDir(uri, getCacheDir(), getContentResolver());

                Intent launchIntent = getPackageManager().getLaunchIntentForPackage(getPackageName());
                launchIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(launchIntent);

                finish();

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
}
