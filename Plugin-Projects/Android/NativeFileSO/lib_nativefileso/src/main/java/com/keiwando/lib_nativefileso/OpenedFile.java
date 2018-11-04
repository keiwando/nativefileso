package com.keiwando.lib_nativefileso;

public class OpenedFile {

    private String filename = "";
    private byte[] data = new byte[0];
    private String path = "";

    public OpenedFile(String filename, byte[] data) {
        this.filename = filename;
        this.data = data;
    }

    public OpenedFile(String filename, byte[] data, String path) {
        this(filename, data);
        this.path = path;
    }

    public byte[] getData() {
        return data;
    }

    public String getFilename() {
        return filename;
    }

    public String getPath() { return path; }
}
