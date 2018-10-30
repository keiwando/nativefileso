using System;
using Keiwando.NativeFileSO;

public class CustomFileTypes {

	public static readonly SupportedFileType evol = new SupportedFileType {
		Name = "Evolution Save File",
		Extension = "evol",
		Owner = true,
		AppleUTI = "com.keiwando.Evolution.evol",
		AppleConformsToUTI = "public.plain-text|public.data",
		MimeType = "text/plain"
	};

	public static readonly SupportedFileType creat = new SupportedFileType {
		Name = "Evolution Creature Save File",
		Extension = "creat",
		Owner = true,
		AppleUTI = "com.keiwando.Evolution.creat",
		AppleConformsToUTI = "public.plain-text|public.data",
		MimeType = "text/plain"
	};
}

