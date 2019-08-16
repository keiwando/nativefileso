// NativeFileSO.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

PWSTR OpenFile() {
	
	HRESULT hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED |
		COINIT_DISABLE_OLE1DDE);
	if (SUCCEEDED(hr)) {
		
		IFileOpenDialog *openDialog;

		hr = CoCreateInstance(CLSID_FileOpenDialog, NULL, CLSCTX_ALL,
			IID_IFileOpenDialog, reinterpret_cast<void**>(&openDialog));

		if (SUCCEEDED(hr)) {
			
			// Show the Open dialog box
			hr = openDialog->Show(NULL);

			if (SUCCEEDED(hr)) {
				
				IShellItem *item;
				hr = openDialog->GetResult(&item);

				if (SUCCEEDED(hr)) {
					
					PWSTR filePath;
					hr = item->GetDisplayName(SIGDN_FILESYSPATH, &filePath);

					if (SUCCEEDED(hr)) {
					
						MessageBox(NULL, filePath, L"File Path", MB_OK);
						item->Release();
						openDialog->Release();
						CoUninitialize();
						return filePath;
					}
					item->Release();
				}
				openDialog->Release();
			}
			CoUninitialize();
		}
		return NULL;
	}
}
