﻿using Microsoft.Win32;
using studyCenter_BL_;
using StudyCenter_DataAccessLayer.Global_classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Study_center.Global_Classes
{
    public class clsGlobal
    {
        public static clsUser CurrentUser;

        public static int Rows => _rowsNumber();
      
        private static int _rowsNumber(){
            var settings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            int _rowsPerPage =0;

          if (short.TryParse(settings["RowsPerPage"], out short value))
                _rowsPerPage = value;

            return _rowsPerPage;
        }
     
        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\StudyCenter";

            string UsernameName = "Username";
            string UsernameData = Username;

            string PasswordName = "Password";
            string PasswordData = Password;

            try
            {
                // Write the value to the Registry
                Registry.SetValue(keyPath, UsernameName, UsernameData, RegistryValueKind.String);
                Registry.SetValue(keyPath, PasswordName, PasswordData, RegistryValueKind.String);

                return true;
            }
            catch (Exception ex)
            {
                clsErrorLogger.LogError(ex); 
                return false;
            }
        }

        public static bool RemoveStoredCredential()
        {
            string keyPath = @"SOFTWARE\StudyCenter";

            string UsernameName = "Username";
            string PasswordName = "Password";

            try
            {
                // Open the registry key in read/write mode with explicit registry view
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey key = baseKey.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            // Delete the specified value
                            key.DeleteValue(UsernameName);
                            key.DeleteValue(PasswordName);

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                clsErrorLogger.LogError(ex);
                return false;
            }
            catch (Exception ex)
            {
                clsErrorLogger.LogError(ex);
                return false;
            }
        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\StudyCenter";

            string UsernameName = "Username";
            string PasswordName = "Password";

            try
            {
                // Read the value from the Registry
                Username = Registry.GetValue(keyPath, UsernameName, null) as string;
                Password = Registry.GetValue(keyPath, PasswordName, null) as string;

                return true;
            }
            catch (Exception ex)
            {
                clsErrorLogger.LogError( ex);
                return false;
            }
        }

        public static string ComputeHash(string input)
        {
            // Create an instance of the SHA-256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value from the UTF-8 encoded input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));


                // Convert the byte array to a lowercase hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string Encrypt(string plainText, string key = "02D2E9-830F-4B31-89C6-237F4131BC")
        {
            if (plainText == null)
            {
                return null;
            }

            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES encryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create an encryptor
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                // Encrypt the data
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }


                    // Return the encrypted data as a Base64-encoded string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key = "02D2E9-830F-4B31-89C6-237F4131BC")
        {
            if (cipherText == null)
            {
                return null;
            }

            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES decryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                // Decrypt the data
                using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    // Read the decrypted data from the StreamReader
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}