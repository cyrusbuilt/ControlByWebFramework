using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace CyrusBuilt.ControlByWeb.Security
{
    /// <summary>
    /// Provides cryptographic utility methods.
    /// </summary>
    public static class Crypto
    {
        #region Fields
        private static readonly String _key = "1a45FgH8(6%2^u/BBp7;v}+";
        #endregion

        #region Methods
        /// <summary>
        /// Encodes the specified input string using the Base64 algorithm.
        /// </summary>
        /// <param name="input">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// A Base64-encoded string. If the input is null or empty,
        /// then an empty string is returned instead.
        /// </returns>
        public static String Base64Encode(String input) {
            if (String.IsNullOrEmpty(input)) {
                return String.Empty;
            }
            Byte[] buffer = ASCIIEncoding.ASCII.GetBytes(input);
            String result = Convert.ToBase64String(buffer);
            Array.Clear(buffer, 0, buffer.Length);
            return result;
        }

        /// <summary>
        /// Converts a plain-text string into a read-only SecureString Datatype.
        /// </summary>
        /// <param name="plainString">
        /// The plain-text string to convert.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="SecureString"/> holding the converted string (read-only).
        /// </returns>
        public static SecureString ConvertToSecureString(String plainString) {
            // Garbage in, garbage out.
            if (String.IsNullOrEmpty(plainString)) {
                return null;
            }

            SecureString result = new SecureString();
            for (Int32 i = 0; i < plainString.Length; i++) {
                result.AppendChar(plainString[i]);
            }
            plainString = null;
            result.MakeReadOnly();
            return result;
        }

        /// <summary>
        /// Converts a <see cref="SecureString"/> value into a plain-text string.
        /// </summary>
        /// <param name="input">
        /// The input to convert.
        /// </param>
        /// <returns>
        /// A plain-text version of the <see cref="SecureString"/>.
        /// </returns>
        public static String ConvertToPlainString(SecureString input) {
            String insecure = String.Empty;
            if (input != null) {
                IntPtr ptr = Marshal.SecureStringToBSTR(input);
                try {
                    insecure = Marshal.PtrToStringBSTR(ptr);
                }
                finally {
                    Marshal.ZeroFreeBSTR(ptr);
                }
            }
            return insecure;
        }

        /// <summary>
        /// Creates an MD5 hash from a plain-text Username and password, and specifed salt.
        /// </summary>
        /// <param name="username">
        /// The username to generate a hash from.
        /// </param>
        /// <param name="password">
        /// The plain-text password to generate a hash from.
        /// </param>
        /// <param name="salt">
        /// The "salt" or key.
        /// </param>
        /// <returns>
        /// An MD5 hash (byte array) containing the hash values.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// None of the parameters can be null or empty.
        /// </exception>
        public static Byte[] GeneratePasswordHash(String username, String password, String salt) {
            if (String.IsNullOrEmpty(username)) {
                throw new ArgumentNullException("username");
            }

            if (String.IsNullOrEmpty(password)) {
                throw new ArgumentNullException("password");
            }

            if (String.IsNullOrEmpty(salt)) {
                throw new ArgumentNullException("salt");
            }

            Byte[] buffer;
            Byte[] ret;
            using (MemoryStream stream = new MemoryStream()) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(salt);
                    writer.Write(username);
                    writer.Write(password);
                    writer.Flush();
                    writer.Close();

                    buffer = stream.ToArray();
                    stream.Close();
                }
            }

            MD5 hash = MD5.Create();
            ret = hash.ComputeHash(buffer);
            Array.Clear(buffer, 0, buffer.Length);
            hash.Clear();
            return ret;
        }

        /// <summary>
        /// Compares 2 MD5 hash values to determine if they match.
        /// </summary>
        /// <param name="hash1">
        /// The first hash to compare (to hash2).
        /// </param>
        /// <param name="hash2">
        /// The second hash to compare (to hash1).
        /// </param>
        /// <returns>
        /// true if the hashes are equal; Otherwise, false.
        /// </returns>
        public static Boolean ComparePasswordHash(Byte[] hash1, Byte[] hash2) {
            // If both values are null, this is still a match.
            if ((hash1 == null) && (hash2 == null)) {
                return true;
            }

            // If only one of the values is null, then they don't match.
            if (((hash1 == null) && (hash2 != null)) ||
                ((hash1 != null) && (hash2 == null))) {
                return false;
            }

            // Compare hash lengths, then do a per-byte comparison of the hash.
            Boolean result = true;
            if (hash1.Length != hash2.Length) {
                result = false;
            }
            else {
                for (Int32 i = 0; i < hash1.Length; i++) {
                    if (hash1[i] != hash2[i]) {
                        result = false;
                    }
                }
            }
            hash1 = null;
            hash2 = null;
            return result;
        }

        /// <summary>
        /// Convert a plain-text string into a byte array.
        /// </summary>
        /// <param name="input">
        /// The string value to convert.
        /// </param>
        /// <returns>
        /// An array of bytes that comprise the string value. If input
        /// is null or empty, returns null instead.
        /// </returns>
        public static Byte[] ConvertStringToByteArray(String input) {
            if (String.IsNullOrEmpty(input)) {
                return null;
            }
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(input);
        }

        /// <summary>
        /// Convert a byte array into plain-text string.
        /// </summary>
        /// <param name="input">
        /// The byte array to convert.
        /// </param>
        /// <returns>
        /// The newly converted string value. If input is null
        /// or empty, returns an empty string.
        /// </returns>
        public static String ConvertByteArrayToString(Byte[] input) {
            if ((input == null) || (input.Length == 0)) {
                return String.Empty;
            }
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(input);
        }

        /// <summary>
        /// Encrypts a string value using the Triple DES algorithm and an MD5 hash.
        /// </summary>
        /// <param name="plainString">
        ///  The string value to encrypt.
        /// </param>
        /// <returns>
        /// The resulting encrypted string.
        /// </returns>
        public static String EncryptString(String plainString) {
            if (String.IsNullOrEmpty(plainString)) {
                return String.Empty;
            }

            String result = String.Empty;
            Byte[] encResult;
            UTF8Encoding utf8 = new UTF8Encoding();

            // First, hash the password using MD5.
            // The result of the MD5 hash generator is a 128bit byte array
            // which is a valid length for the TripleDES encoder.
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            Byte[] TDESKey = hashProvider.ComputeHash(utf8.GetBytes(_key));

            // Create and setup the encoder.
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = TDESKey;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            // Convert PlainString to a byte array.
            Byte[] dataToEncrypt = utf8.GetBytes(plainString);

            // Attempt encryption.
            try {
                ICryptoTransform encryptor = des.CreateEncryptor();
                encResult = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            catch (Exception) {
                encResult = null;
            }
            finally {
                des.Clear();
                hashProvider.Clear();
            }

            TDESKey = null;
            dataToEncrypt = null;
            if (encResult != null) {
                // If successful the final product is converted to Base64.
                result = Convert.ToBase64String(encResult);
                encResult = null;
            }
            return result;
        }

        /// <summary>
        /// Decrypts a string value that was encrypted by the <see cref="EncryptString"/> method.
        /// </summary>
        /// <param name="input">
        /// The encrypted string to decrypt.
        /// </param>
        /// <returns>
        /// The resulting decrypted string value.
        /// </returns>
        public static String DecryptString(String input) {
            if (String.IsNullOrEmpty(input)) {
                return String.Empty;
            }

            String result = String.Empty;
            Byte[] decResult;
            UTF8Encoding utf8 = new UTF8Encoding();

            // First, hash the password using MD5.
            // The result of the MD5 hash generator is a 128bit byte array
            // which is a valid length for the TripleDES decoder.
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            Byte[] TDESKey = hashProvider.ComputeHash(utf8.GetBytes(_key));

            // Create and setup the decoder.
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = TDESKey;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            // Convert the input string to a byte array.
            Byte[] dataToDecrypt = Convert.FromBase64String(input);

            // Attempt decryption.
            try {
                ICryptoTransform decryptor = des.CreateDecryptor();
                decResult = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            catch (Exception) {
                decResult = null;
            }
            finally {
                des.Clear();
                hashProvider.Clear();
            }

            TDESKey = null;
            dataToDecrypt = null;
            if (decResult != null) {
                // If successful, then convert the product to a string.
                result = utf8.GetString(decResult);
                decResult = null;
            }
            return result;
        }
        #endregion
    }
}
