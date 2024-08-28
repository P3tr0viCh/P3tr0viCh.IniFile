using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace P3tr0viCh.IniFile
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string value, string lpFileName);

        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int lpDefault, string lpFileName);

        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);


        public string FileName { get; set; }

        public IniFile(string fileName)
        {
            FileName = fileName;
        }

        public void ReadSections(List<string> sections)
        {
            uint MAX_BUFFER = 32767;

            var lpszReturnBuffer = Marshal.AllocCoTaskMem((int)MAX_BUFFER);

            var bytesReturned = GetPrivateProfileSectionNames(lpszReturnBuffer, MAX_BUFFER, FileName);

            if (bytesReturned == 0)
            {
                return;
            }

            string local = Marshal.PtrToStringAnsi(lpszReturnBuffer, (int)bytesReturned).ToString();

            Marshal.FreeCoTaskMem(lpszReturnBuffer);

            sections.AddRange(local.Substring(0, local.Length - 1).Split('\0'));
        }

        public bool SectionExists(string section)
        {
            var sections = new List<string>();

            ReadSections(sections);

            return sections.Contains(section);
        }

        public void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, FileName);
        }

        public string ReadString(string section, string key, string def = default)
        {
            var builder = new StringBuilder(255);

            GetPrivateProfileString(section, key, def, builder, 255, FileName);

            return builder.ToString();
        }

        public void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
        }

        public int ReadInteger(string section, string key, int def = default)
        {
            return GetPrivateProfileInt(section, key, def, FileName);
        }

        public void WriteFloat(string section, string key, float value)
        {
            WriteString(section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public float ReadFloat(string section, string key, float def = default)
        {
            float.TryParse(
                ReadString(section, key, def.ToString(CultureInfo.InvariantCulture)),
                NumberStyles.Float, CultureInfo.InvariantCulture, out float result);

            return result;
        }

        public void WriteDateTime(string section, string key, DateTime value)
        {
            WriteString(section, key, value.ToString());
        }

        public DateTime ReadDateTime(string section, string key, DateTime def = default)
        {
            DateTime.TryParse(ReadString(section, key, def.ToString()), out DateTime result);

            return result;
        }

        public void WriteBool(string section, string key, bool value)
        {
            WriteInteger(section, key, value ? 1 : 0);
        }

        public bool ReadBool(string section, string key, bool def = default)
        {
            return ReadInteger(section, key, def ? 1 : 0) == 1;
        }
    }
}