package com.keiwando.lib_nativefileso;

public class OpenedFile {

    private String filename = "";
    private byte[] data = new byte[0];

    public OpenedFile(String filename, byte[] data) {
        this.filename = filename;
        this.data = data;
    }

    public byte[] getData() {
        return data;
    }

    public String getFilename() {
        return filename;
    }
}
