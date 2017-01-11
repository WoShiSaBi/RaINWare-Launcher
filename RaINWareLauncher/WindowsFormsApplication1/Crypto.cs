﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    public static string Encrypt(string clearText)
    {
        var EncryptionKey = "33eG9*aRZg)$?hAgS";
        var clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (var encryptor = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }
}