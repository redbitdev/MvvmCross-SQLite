// MvxWindowsPhoneSQLiteConnectionFactory.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System.IO;
using Community.SQLite;


namespace Cirrious.MvvmCross.Community.Plugins.Sqlite.WindowsPhone
{
    public class MvxWindowsPhoneSQLiteConnectionFactory
        : ISQLiteConnectionFactory
        , ISQLiteConnectionFactoryEx
    {
        public ISQLiteConnection Create(string address)
        {
            return CreateEx(address);
        }

        public ISQLiteConnection CreateEx(string address, SQLiteConnectionOptions options = null)
        {
            options = options ?? new SQLiteConnectionOptions();
            var path = options.BasePath ?? string.Empty;
            var filePath = Path.Combine(path, address);
            return SQLiteConnection.Create(filePath, options);
        }
    }
}