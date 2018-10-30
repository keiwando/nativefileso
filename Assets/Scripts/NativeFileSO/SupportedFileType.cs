using System;
namespace Keiwando.NativeFileSO {

	public class SupportedFileType {

		/// <summary>
		/// The title of this file type.
		/// </summary>
		public string Name;

		/// <summary>
		/// The file extension.
		/// </summary>
		public string Extension;

		/// <summary>
		/// Specifies whether this application is the primary creator
		/// of this file type. (e.g. custom save files)
		/// </summary>
		public bool Owner;

		/// <summary>
		/// The Uniform Type Identifier used for file association on iOS.
		/// See https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/understanding_utis/understand_utis_declare/understand_utis_declare.html#//apple_ref/doc/uid/TP40001319-CH204-SW1
		/// for more info.
		/// 
		/// Separate multiple UTIs with a pipe (|).
		/// </summary>
		public string AppleUTI;


		/// <summary>
		/// Set this value if you are declaring a custom file type that conforms
		/// to an existing UTI. 
		/// 
		/// For example, if you have a custom text file type, set this value
		/// to public.plain-text
		/// 
		/// Separate multiple UTIs with a pipe (|).
		/// </summary>
		public string AppleConformsToUTI = "";

		/// <summary>
		/// The MimeType of the file. On Android, only the MimeType is relevant
		/// for filtering supported files.
		/// </summary>
		public string MimeType;

		// Presets
		public static readonly SupportedFileType Any = new SupportedFileType {

			Name = "Any",
			Extension = "*",
			Owner = false,
			AppleUTI = "public.item|public.content",
			MimeType = "*/*"
		};

		public static readonly SupportedFileType PlainText = new SupportedFileType {

			Name = "Text File",
			Extension = "txt",
			Owner = false,
			AppleUTI = "public.plain-text",
			MimeType = "text/plain"
		};

		public static readonly SupportedFileType XML = new SupportedFileType {

			Name = "XML File",
			Extension = "xml",
			Owner = false,
			AppleUTI = "public.text",
			MimeType = "text/xml"
		};

		public static readonly SupportedFileType JPEG = new SupportedFileType {

			Name = "JPEG Image",
			Extension = "jpg",
			Owner = false,
			AppleUTI = "public.jpeg",
			MimeType = "image/jpeg"
		};

		public static readonly SupportedFileType PNG = new SupportedFileType {

			Name = "PNG Image",
			Extension = "png",
			Owner = false,
			AppleUTI = "public.png",
			MimeType = "image/png"
		};

		public static readonly SupportedFileType MP4 = new SupportedFileType {

			Name = "MPEG-4 content",
			Extension = "mp4",
			Owner = false,
			AppleUTI = "public.mpeg-4",
			MimeType = "video/mp4"
		};

		public static readonly SupportedFileType MP3 = new SupportedFileType {

			Name = "MPEG-3 audio",
			Extension = "mp3",
			Owner = false,
			AppleUTI = "public.mp3",
			MimeType = "audio/mpeg3"
		};

		public static readonly SupportedFileType PDF = new SupportedFileType {

			Name = "PDF",
			Extension = "pdf",
			Owner = false,
			AppleUTI = "com.adobe.pdf",
			MimeType = "application/pdf"
		};

		public static readonly SupportedFileType GIF = new SupportedFileType {

			Name = "Gif",
			Extension = "gif",
			Owner = false,
			AppleUTI = "com.compuserve.gif",
			MimeType = "image/gif"
		};
	}
}