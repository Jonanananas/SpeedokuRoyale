using System.Security.Cryptography;
using System;

using UnityEngine;

public static class HashPassword {
    const int saltSize = 16; // 128 bits
    const int hashSize = 20; // 160 bits
    const int iterations = 100000;

    public static byte[] Hash(string password) {
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(hashSize);

        byte[] hashBytes = new byte[saltSize + hashSize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

        // Verify test:
        // string passwordHash = Convert.ToBase64String(hashBytes);
        // Trace.Log((Verify(password, passwordHash)).ToString());
        // Trace.Log("passwordHash: " + passwordHash);

        return hashBytes;
    }

    public static bool Verify(string password, string passwordHash) {
        // Extract the bytes
        byte[] hashBytes = Convert.FromBase64String(passwordHash);
        // Get the salt
        byte[] salt = new byte[saltSize];
        Array.Copy(hashBytes, 0, salt, 0, saltSize);
        // Compute the hash on the password the user entered 
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(hashSize);
        // Compare the results 
        for (int i = 0; i < hashSize; i++)
            if (hashBytes[i + saltSize] != hash[i])
                return false;
        return true;
    }
}
