using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using NUnit.Framework;

#if __IOS__
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Touch;
#elif __ANDROID__
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid;
#else
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Wpf;
#endif

namespace TestSqliteWpf
{
    [TestFixture]
    public class CreateDatabaseTest
    {
        [Test]
        public void ShouldCreateFileDatabase()
        {
            // Items Needing Cleanup
            ISQLiteConnection conn = null;
            string expectedFilePath = null;
            try
            {
                // Arrange
#if __IOS__
                ISQLiteConnectionFactory factory = new MvxTouchSQLiteConnectionFactory();
#elif __ANDROID__
                ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();
#else
                ISQLiteConnectionFactory factory = new MvxWpfSqLiteConnectionFactory();
#endif
                
                string filename = Guid.NewGuid().ToString() + ".db";
#if __IOS__ || __ANDROID__
                expectedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
#else
                expectedFilePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
#endif                

                // Act
                conn = factory.Create(filename);
                conn.CreateTable<Person>();
                conn.Insert(new Person() { FirstName = "Bob", LastName = "Smith" });
                Person expected = conn.Table<Person>().FirstOrDefault();

                // Asset
                Assert.That(File.Exists(expectedFilePath), Is.True);
                Assert.That(expected.FirstName, Is.EqualTo("Bob"));
                Assert.That(expected.LastName, Is.EqualTo("Smith"));
            }
            finally // Cleanup in Finally
            {
                if (conn != null)
                    conn.Close();
                
                if (!string.IsNullOrWhiteSpace(expectedFilePath) && File.Exists(expectedFilePath))
                    File.Delete(expectedFilePath);
            }
        }

        [Test]
        public void ShouldCreateInMemoryDatabase()
        {
            // Items Needing Cleanup
            ISQLiteConnection conn = null;

            try
            {
                // Arrange
#if __IOS__
                ISQLiteConnectionFactory factory = new MvxTouchSQLiteConnectionFactory();
#elif __ANDROID__
                ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();
#else
                ISQLiteConnectionFactory factory = new MvxWpfSqLiteConnectionFactory();
#endif

                // Act
                conn = factory.CreateInMemory();
                conn.CreateTable<Person>();
                conn.Insert(new Person() { FirstName = "Bob", LastName = "Smith" });
                Person expected = conn.Table<Person>().FirstOrDefault();

                // Asset
                Assert.That(expected.FirstName, Is.EqualTo("Bob"));
                Assert.That(expected.LastName, Is.EqualTo("Smith"));
            }
            finally // Cleanup in Finally
            {
                if (conn != null)
                    conn.Close();
                
            }
        }

        [Test]
        public void ShouldCreateTempDatabase()
        {
            // Items Needing Cleanup
            ISQLiteConnection conn = null;

            try
            {
                // Arrange
#if __IOS__
                ISQLiteConnectionFactory factory = new MvxTouchSQLiteConnectionFactory();
#elif __ANDROID__
                ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();
#else
                ISQLiteConnectionFactory factory = new MvxWpfSqLiteConnectionFactory();
#endif

                // Act
                conn = factory.CreateTemp();
                conn.CreateTable<Person>();
                conn.Insert(new Person() { FirstName = "Bob", LastName = "Smith" });
                Person expected = conn.Table<Person>().FirstOrDefault();

                // Asset
                Assert.That(expected.FirstName, Is.EqualTo("Bob"));
                Assert.That(expected.LastName, Is.EqualTo("Smith"));
            }
            finally // Cleanup in Finally
            {
                if (conn != null)
                    conn.Close();

            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), MatchType = MessageMatch.Contains, ExpectedMessage="option")]
        public void ShouldThrowArgumentNullExceptionOnCreateGivenNullOptions()
        {
            // Items Needing Cleanup
            ISQLiteConnection conn = null;

            try
            {
                // Arrange
#if __IOS__
                ISQLiteConnectionFactoryEx factory = new MvxTouchSQLiteConnectionFactory();
#elif __ANDROID__
                ISQLiteConnectionFactoryEx factory = new MvxDroidSQLiteConnectionFactory();
#else
                ISQLiteConnectionFactoryEx factory = new MvxWpfSqLiteConnectionFactory();
#endif

                // Act
                conn = factory.CreateEx(null);
            }
            finally // Cleanup in Finally
            {
                if (conn != null) // In case test fails and connection was created
                    conn.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), MatchType = MessageMatch.Contains, ExpectedMessage = "Address cannot be empty or null")]
        public void ShouldThrowArgumentExceptionOnCreateGivenOptionsWithNullAddressAndTypeFile()
        {
            // Items Needing Cleanup
            ISQLiteConnection conn = null;

            try
            {
                // Arrange
#if __IOS__
                ISQLiteConnectionFactoryEx factory = new MvxTouchSQLiteConnectionFactory();
#elif __ANDROID__
                ISQLiteConnectionFactoryEx factory = new MvxDroidSQLiteConnectionFactory();
#else
                ISQLiteConnectionFactoryEx factory = new MvxWpfSqLiteConnectionFactory();
#endif
                SQLiteConnectionOptions options = new SQLiteConnectionOptions { Address = null, Type = SQLiteConnectionOptions.DatabaseType.File };

                // Act                
                conn = factory.CreateEx(options);
            }
            finally // Cleanup in Finally
            {
                if (conn != null) // In case test fails and connection was created
                    conn.Close();
            }
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
