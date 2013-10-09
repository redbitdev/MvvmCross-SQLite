// MvxTouchSQLiteConnectionFactory.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.IO;
using Community.SQLite;

namespace Cirrious.MvvmCross.Community.Plugins.Sqlite.Touch
{
    public class MvxTouchSQLiteConnectionFactory
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
            var path = options.BasePath ?? Environment.GetFolderPath(Environment.SpecialFolder.Personal);Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(path, address);
            return new SQLiteConnection(filePath, options.StoreDateTimeAsTicks);
        }
    }
}