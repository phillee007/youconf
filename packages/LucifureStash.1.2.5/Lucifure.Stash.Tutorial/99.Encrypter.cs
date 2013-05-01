using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace CodeSuperior.Lucifure.Tutorial
{
	/// <summary>
	/// Please note that Encrypter is for demo purposes only and should not be considered as production quality.
	/// </summary>
	public 
	class Encrypter
	{
			ICryptoTransform					_encryptor;
			ICryptoTransform					_decryptor;

		public
		Encrypter(
		    string								password,
		    string								salt)
		{
			

			byte[]
		    saltBytes = UTF8Encoding.UTF8.GetBytes(
											salt.Length < 8 
												?	(salt + "Lucifure").Substring(0, 8)	// force to 8 bytes at least
												:	salt);	

		    Rfc2898DeriveBytes
		    rfc = new Rfc2898DeriveBytes(password, saltBytes, 1000);

		    // key is 256 bits = 32 bytes
		    byte[]
			key = rfc.GetBytes(32);

		    // initialization vector is 16 bytes
		    byte[]
			iv = new byte[16];

		    // iv is 128 bits = 16 bytes
		    Array.Copy(key, 7, iv, 0, 16);

		    Create(key, iv);
		}

		public
		Encrypter(
			byte[]								key,
			byte[]								iv)
		{
			Create(key, iv);
		}

		void
		Create(
			byte[]								key,
			byte[]								iv)
		{
			AesManaged
			aes = new AesManaged();

			_encryptor = aes.CreateEncryptor(key, iv);
			_decryptor = aes.CreateDecryptor(key, iv);
		}


		public
		byte[]
		Encrypt(
			string								data)
		{
			return Encrypt(UTF8Encoding.UTF8.GetBytes(data));
		}

		public
		byte[]
		Encrypt(
			byte[]								data)
		{
			byte[]								result = null;

			using (MemoryStream encryptionStream = new MemoryStream())
			{
				// Create the crypto stream
				using (CryptoStream encrypt = new CryptoStream(
														encryptionStream, 
														_encryptor, 
														CryptoStreamMode.Write))
				{
					encrypt.Write(data, 0, data.Length);
					encrypt.FlushFinalBlock();

					encrypt.Clear();
					//encrypt.Close(); 
				}

				result = encryptionStream.ToArray();
			}

			return result;
		}

		public
		string
		DecryptToString(
			byte[]								data)
		{
			byte[]								unencrypted;

			return UTF8Encoding.UTF8.GetString(
										unencrypted = Decrypt(data),
										0,
										unencrypted.Length); 
		}

		public
		byte[]
		Decrypt(
			byte[]								data)
		{
			byte[]								result = null;

			using (MemoryStream encryptionStream = new MemoryStream())
			{
				// Create the crypto stream
				using (CryptoStream encrypt = new CryptoStream(
														encryptionStream, 
														_decryptor, 
														CryptoStreamMode.Write))
				{
					encrypt.Write(data, 0, data.Length);
					encrypt.FlushFinalBlock();

					encrypt.Clear();
					//encrypt.Close(); 
				}

				result = encryptionStream.ToArray();
			}

			return result;
		}
	}
}