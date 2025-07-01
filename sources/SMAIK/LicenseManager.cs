using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

[Serializable]
public class LicenseData
{
    public string UserName { get; set; }             // Имя пользователя
    public string UserProfile { get; set; }          // Тип лицензии: "Demo" / "Full"
    public bool disableEditGr { get; set; }
    public bool disablePlugin { get; set; }
    public bool disablEditPublication { get; set; }
    public DateTime ExpirationDate { get; set; }     // Дата окончания лицензии


}

[Serializable]
public class LicenseContainer
{
    public LicenseData License { get; set; }
    public string UsbSerial { get; set; }

    public byte[] Serialize()
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms, Encoding.UTF8))
        {
            writer.Write(License.UserName ?? string.Empty);
            writer.Write(License.UserProfile ?? string.Empty);
            writer.Write(License.disableEditGr);
            writer.Write(License.disablePlugin);
            writer.Write(License.disablEditPublication);
            writer.Write(License.ExpirationDate.ToBinary());

            writer.Write(UsbSerial ?? string.Empty);

            return ms.ToArray();
        }
    }

    public static LicenseContainer Deserialize(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms, Encoding.UTF8))
        {
            return new LicenseContainer
            {
                License = new LicenseData
                {
                    UserName = reader.ReadString(),
                    UserProfile = reader.ReadString(),
                    disableEditGr = reader.ReadBoolean(),
                    disablePlugin = reader.ReadBoolean(),
                    disablEditPublication = reader.ReadBoolean(),
                    ExpirationDate = DateTime.FromBinary(reader.ReadInt64())
                },
                UsbSerial = reader.ReadString()
            };
        }
    }
}



    public class LicenseManager
{
    private const string EncryptionSalt = "someSalt";
    private static LicenseContainer _currentLicense;
    private static DateTime _programStartTime;
    public static int _usedMinutes = 0;
    private static System.Windows.Forms.Timer _usageTimer;
    private static string _usbSerialToCheck;
    private DateTime expirationDate;

    public static bool IsFullVersion => _currentLicense?.License.UserProfile == "admin";
    public static bool disableEditGr => _currentLicense?.License.disableEditGr ?? true;
    public static bool disablEditPublication => _currentLicense?.License.disablEditPublication ?? true;
    public static bool disablePlugin => _currentLicense?.License.disablePlugin ?? true;
    public static DateTime ExpirationDate => _currentLicense?.License.ExpirationDate ?? DateTime.MaxValue;
    public static string CurrentUserName => _currentLicense?.License.UserName ?? "Nobody User";
    public static string CurrentUserProfile => _currentLicense?.License.UserProfile ?? "Content-maker";

    public static void Initialize()
    {
        CheckForLicense();
    }

    public static void CheckForLicense()
    {
        try
        {
            var usbDrives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable && d.IsReady)
                .ToList();

            foreach (var drive in usbDrives)
            {
                string licensePath = Path.Combine(drive.RootDirectory.FullName, "license.lic");
                if (File.Exists(licensePath))
                {
                    _usbSerialToCheck = GetUsbSerialNumber(drive.Name[0].ToString());
                    if (string.IsNullOrEmpty(_usbSerialToCheck))
                        continue;

                    byte[] licenseData = File.ReadAllBytes(licensePath);
                    if (TryDecryptLicense(licenseData, out var licenseContainer))
                    {
                        if (_usbSerialToCheck != licenseContainer.UsbSerial)
                        {
                            continue;
                        }
                        
                        if (licenseContainer.License.ExpirationDate < DateTime.Now)
                        {
                            MessageBox.Show("Срок действия лицензии истек.");
                            continue;
                        }

                        _currentLicense = licenseContainer;
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка проверки лицензии: {ex.Message}");
        }

        //MessageBox.Show("Лицензия не была найдена. Программа будет запущена в демо-режиме с максимальными ограничениями.");
        _currentLicense = null;
        return;
    }

    private static bool TryDecryptLicense(byte[] licenseKey, out LicenseContainer container)
    {
        container = null;

        try
        {
            // Извлекаем IV (первые 16 байт)
            byte[] iv = new byte[16];
            Buffer.BlockCopy(licenseKey, 0, iv, 0, iv.Length);

            // Извлекаем зашифрованные данные
            int encryptedLength = licenseKey.Length - iv.Length - 32;
            byte[] encryptedData = new byte[encryptedLength];
            Buffer.BlockCopy(licenseKey, iv.Length, encryptedData, 0, encryptedLength);

            // Извлекаем хеш
            byte[] storedHash = new byte[32];
            Buffer.BlockCopy(licenseKey, iv.Length + encryptedLength, storedHash, 0, 32);

            if (string.IsNullOrEmpty(_usbSerialToCheck))
                return false;

            // Расшифровываем данные
            byte[] decryptedData;
            using (var aes = Aes.Create())
            {
                using (var deriveBytes = new Rfc2898DeriveBytes(_usbSerialToCheck, Encoding.UTF8.GetBytes(EncryptionSalt), 1000))
                {
                    aes.Key = deriveBytes.GetBytes(32);
                }

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(encryptedData))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var output = new MemoryStream())
                {
                    cs.CopyTo(output);
                    decryptedData = output.ToArray();
                }
            }

            // Проверяем целостность
            using (var sha256 = SHA256.Create())
            {
                byte[] computedHash = sha256.ComputeHash(decryptedData);
                if (!computedHash.SequenceEqual(storedHash))
                    return false;
            }

            // Используем нашу новую десериализацию
            container = LicenseContainer.Deserialize(decryptedData);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка дешифровки лицензии: {ex.Message}");
            return false;
        }
    }

    private static string GetUsbSerialNumber(string driveLetter)
    {
        try
        {
            // Получаем все USB-диски
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
            {
                foreach (ManagementObject disk in searcher.Get())
                {
                    try
                    {
                        // Получаем связанные логические диски
                        using (var logicalSearcher = new ManagementObjectSearcher(
                            $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{disk["DeviceID"]}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition"))
                        {
                            foreach (ManagementObject partition in logicalSearcher.Get())
                            {
                                using (var driveSearcher = new ManagementObjectSearcher(
                                    $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass=Win32_LogicalDiskToPartition"))
                                {
                                    foreach (ManagementObject logicalDisk in driveSearcher.Get())
                                    {
                                        string diskPath = logicalDisk["DeviceID"]?.ToString();
                                        if (!string.IsNullOrEmpty(diskPath) && diskPath.StartsWith(driveLetter, StringComparison.OrdinalIgnoreCase))
                                        {
                                            string pnpDeviceId = disk["PNPDeviceID"]?.ToString();
                                            if (!string.IsNullOrEmpty(pnpDeviceId))
                                            {
                                                return Retrieve_serial(pnpDeviceId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        disk.Dispose();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка получения серийного номера: {ex.Message}");
        }
        return null;
    }

    private static string Retrieve_serial(string strSource)
    {
        if (string.IsNullOrEmpty(strSource)) return null;

        string strStart = "\\";
        int Start, End;
        Start = strSource.LastIndexOf(strStart) + strStart.Length;
        End = strSource.IndexOf("&0", Start);

        if (Start < 0 || End <= Start) return null;

        string serial = strSource.Substring(Start, End - Start);
        return serial;
    }

    
}