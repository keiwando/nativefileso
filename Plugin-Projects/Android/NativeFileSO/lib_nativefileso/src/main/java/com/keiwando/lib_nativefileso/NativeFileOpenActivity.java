package com.keiwando.lib_nativefileso;

import android.app.Activity;
import android.content.ClipData;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import com.keiwando.lib_nativefileso.androidx.annotation.Nullable;

import java.util.ArrayList;

public class NativeFileOpenActivity extends Activity {

    private final int REQUEST_CODE = 1;

    @Override
    @SuppressWarnings("unchecked")
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent lastIntent = getIntent();

        if (lastIntent.getBooleanExtra("openedFromNativeFileSO", false)) {

            //Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT); // 4.4+
            Intent intent = new Intent(Intent.ACTION_GET_CONTENT);

            String encodedTypes = lastIntent.getStringExtra("mimetypes");
            intent.setType("*/*");
            intent.putExtra(Intent.EXTRA_LOCAL_ONLY, true);
            boolean canOpenMultiple = lastIntent.getBooleanExtra("canOpenMultiple", false);
            if (canOpenMultiple) {
                intent.putExtra(Intent.EXTRA_ALLOW_MULTIPLE, true); // 4.4+
            }

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

            // File is trying to be opened externally
            ArrayList<Uri> uris = new ArrayList<>();
            String action = lastIntent.getAction();
            if (action == Intent.ACTION_SEND) {
                uris.add((Uri)lastIntent.getExtras().get(Intent.EXTRA_STREAM));

            } else if (action == Intent.ACTION_SEND_MULTIPLE){
                uris = (ArrayList<Uri>)lastIntent.getExtras().get(Intent.EXTRA_STREAM);
            } else if (action == Intent.ACTION_VIEW || action == Intent.ACTION_EDIT) {
                uris.add((Uri)lastIntent.getData());
            } else {
                Log.d("Plugin DEBUG", "Intent action not known.");
            }

            if (uris != null) {

                NativeFileOpenURLBuffer.getInstance().saveFilesInCacheDir(uris, getCacheDir(), getContentResolver());

                Intent launchIntent = getPackageManager().getLaunchIntentForPackage(getPackageName());
                launchIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(launchIntent);

            } else {
                Log.d("Plugin DEBUG", "No Uris");
            }

            finish();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        Log.d("Plugin DEBUG", "OnActivity result " + (data == null));

        if (requestCode == REQUEST_CODE && resultCode == Activity.RESULT_OK) {

            if (data != null) {

                ArrayList<Uri> uris = new ArrayList<>();
                ClipData clipData = data.getClipData();
                if (clipData == null) {

                    uris.add(data.getData());
                } else {
                    for (int i = 0; i < clipData.getItemCount(); i++) {
                        uris.add(clipData.getItemAt(i).getUri());
                    }
                }
                //NativeFileOpenURLBuffer.getInstance().refreshBufferWithUris(uris, getContentResolver());
                NativeFileOpenURLBuffer.getInstance().saveFilesInCacheDir(uris, getCacheDir(), getContentResolver());
            }
        }

        finish();
    }
}
