// Plugin.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid
{
    public class Plugin
        : IMvxPlugin          
    {
        public void Load()
        {
            var factory = new MvxDroidSQLiteConnectionFactory();
            Mvx.RegisterSingleton<ISQLiteConnectionFactory>(factory);
            Mvx.RegisterSingleton<ISQLiteConnectionFactoryEx>(factory);
        }
    }
}