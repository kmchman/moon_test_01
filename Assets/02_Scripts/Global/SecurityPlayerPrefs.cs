using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class SecurityPlayerPrefs
{	
	private string _saltForKey;
	
	private byte[] _keys;
	private byte[] _iv;
	private int keySize 			= 256;
	private int blockSize 			= 128;
	private int _hashLen 			= 32;

	private MD5CryptoServiceProvider md5;

	public SecurityPlayerPrefs()
	{
		byte[] saltBytes 			= new byte[] { 79, 42, 11, 21, 79, 64, 45, 17 };
		string randomSeedForKey 	= "9b6fbs2aaa0a19acae649dae45k506yo";
		string randomSeedForValue 	= "9d170606789841a5ek5c706y6o2au740";

		{
			Rfc2898DeriveBytes key 	= new Rfc2898DeriveBytes(randomSeedForKey, saltBytes, 1000);
			_saltForKey 			= System.Convert.ToBase64String(key.GetBytes(blockSize / 8));
		}

		{
			Rfc2898DeriveBytes key 	= new Rfc2898DeriveBytes(randomSeedForValue, saltBytes, 1000);
			_keys 					= key.GetBytes(keySize / 8);
			_iv 					= key.GetBytes(blockSize / 8);
		}

		md5 = new MD5CryptoServiceProvider();
	}

//	public string MakeHash(string original)
//	{
//		byte[] bytes 		= System.Text.Encoding.UTF8.GetBytes(original);
//		byte[] hashBytes 	= md5.ComputeHash(bytes);
//		
//		string hashToString = "";
//		for (int i = 0; i < hashBytes.Length; ++i)
//		{
//			hashToString 	+= hashBytes[i].ToString(ValueFormat.COMMON_X2);
//		}
//		
//		return hashToString;
//	}
	
	public byte[] Encrypt(byte[] bytesToBeEncrypted)
	{
		using (RijndaelManaged aes = new RijndaelManaged())
		{
			aes.KeySize 	= keySize;
			aes.BlockSize 	= blockSize;
			
			aes.Key 		= _keys;
			aes.IV 			= _iv;
			
			aes.Mode 		= CipherMode.CBC;
			aes.Padding 	= PaddingMode.PKCS7;
			
			using (ICryptoTransform ct = aes.CreateEncryptor())
			{
				return ct.TransformFinalBlock(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
			}
		}
	}
	
	public byte[] Decrypt(byte[] bytesToBeDecrypted)
	{
		using (RijndaelManaged aes = new RijndaelManaged())
		{
			aes.KeySize 	= keySize;
			aes.BlockSize 	= blockSize;
			
			aes.Key 		= _keys;
			aes.IV 			= _iv;
			
			aes.Mode 		= CipherMode.CBC;
			aes.Padding 	= PaddingMode.PKCS7;
			
			using (ICryptoTransform ct = aes.CreateDecryptor())
			{
				return ct.TransformFinalBlock(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
			}
		}
	}
	
	public string Encrypt(string input)
	{
		byte[] bytesToBeEncrypted 	= Encoding.UTF8.GetBytes(input);
		byte[] bytesEncrypted 		= Encrypt(bytesToBeEncrypted);
		
		return System.Convert.ToBase64String(bytesEncrypted);
	}
	
	public string Decrypt(string input)
	{
		byte[] bytesToBeDecrypted 	= System.Convert.FromBase64String(input);
		byte[] bytesDecrypted 		= Decrypt(bytesToBeDecrypted);
		
		return Encoding.UTF8.GetString(bytesDecrypted);
	}
	
//	private void SetSecurityValue(string key, string value)
//	{
//		string hideKey 				= MakeHash(key + _saltForKey);
//		string encryptValue 		= Encrypt(value + MakeHash(value));
//		
//		PlayerPrefs.SetString(hideKey, encryptValue);
//	}
//	
//	private string GetSecurityValue(string key)
//	{
//		string hideKey 				= MakeHash(key + _saltForKey);
//		
//		string encryptValue 		= PlayerPrefs.GetString(hideKey);
//		if (true == string.IsNullOrEmpty(encryptValue))
//		{
//			return string.Empty;
//		}
//		
//		string valueAndHash 		= Decrypt(encryptValue);
//		if (_hashLen > valueAndHash.Length)
//		{
//			return string.Empty;
//		}
//		
//		string savedValue 			= valueAndHash.Substring(0, valueAndHash.Length - _hashLen);
//		string savedHash 			= valueAndHash.Substring(valueAndHash.Length - _hashLen);
//		
//		if (MakeHash(savedValue) != savedHash)
//		{
//			return string.Empty;
//		}
//		
//		return savedValue;
//	}
//	
//	public void DeleteKey(string key)
//	{
//		PlayerPrefs.DeleteKey(MakeHash(key + _saltForKey));
//	}
	
	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}
	
	public void Save()
	{
		PlayerPrefs.Save();
	}
	
//	public void SetInt(string key, int value)
//	{
//		SetSecurityValue(key, value.ToString());
//	}
//	
//	public void SetLong(string key, long value)
//	{
//		SetSecurityValue(key, value.ToString());
//	}
//	
//	public void SetFloat(string key, float value)
//	{
//		SetSecurityValue(key, value.ToString());
//	}
//	
//	public void SetString(string key, string value)
//	{
//		SetSecurityValue(key, value);
//	}
//	
//	public int GetInt(string key, int defaultValue)
//	{
//		string originalValue 		= GetSecurityValue(key);
//		if (true == string.IsNullOrEmpty(originalValue))
//		{
//			return defaultValue;
//		}
//		
//		int result 					= defaultValue;
//		if (false == int.TryParse(originalValue, out result))
//		{
//			return defaultValue;
//		}
//		
//		return result;
//	}
//	
//	public long GetLong(string key, long defaultValue)
//	{
//		string originalValue 		= GetSecurityValue(key);
//		if (true == string.IsNullOrEmpty(originalValue))
//		{
//			return defaultValue;
//		}
//		
//		long result 				= defaultValue;
//		if (false == long.TryParse(originalValue, out result))
//		{
//			return defaultValue;
//		}
//		
//		return result;
//	}
//	
//	public float GetFloat(string key, float defaultValue)
//	{
//		string originalValue 		= GetSecurityValue(key);
//		if (true == string.IsNullOrEmpty(originalValue))
//		{
//			return defaultValue;
//		}
//		
//		float result 				= defaultValue;
//		if (false == float.TryParse(originalValue, out result))
//		{
//			return defaultValue;
//		}
//		
//		return result;
//	}
//	
//	public string GetString(string key)
//	{
//		string originalValue 		= GetSecurityValue(key);
//		if (true == string.IsNullOrEmpty(originalValue))
//		{
//			return null;
//		}
//		
//		return originalValue;
//	}
//	
//	public string GetString(string key, string defaultValue)
//	{
//		string originalValue 		= GetSecurityValue(key);
//		if (true == string.IsNullOrEmpty(originalValue))
//		{
//			return defaultValue;
//		}
//		
//		return originalValue;
//	}
}