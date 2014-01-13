﻿// BaseClasses.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SQLiteOpenFlags = System.Int16;

namespace Cirrious.MvvmCross.Community.Plugins.Sqlite
{
    /// <summary>
    /// This class is used to provide advanced options for creation of
    /// a SQLiteConnection.
    /// </summary>
    public class SQLiteConnectionOptions
    {
        /// <summary>
        /// Constructs a SQLiteConnectionOptions with default settings.
        /// </summary>
		public SQLiteConnectionOptions()
		{
		    // we want the default to be Ticks - see https://github.com/slodge/MvvmCross/issues/213#issuecomment-24610834
			StoreDateTimeAsTicks = true;        
		}
		/// <summary>
		/// The name of the database file.
		/// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The base path1 to use in conjunction with the Address to create 
        /// the full location to store the database file.
        /// </summary>
        /// <remarks>
        /// This is used for determining the name of File databases, and the key
        /// used for distinct in-memory databases. This is not used for temporary
        /// databases.
        /// <para>
        /// Please note that if this value is null it will be set based on best 
        /// know location based on platform. Only provide this value if you 
        /// know what you are doing. If this value is left empty it will not 
        /// be replaced and only the Address will be used.
        /// </para>
        /// </remarks>
        public string BasePath { get; set; }
        /// <summary>
        /// If true will store DateTime properties as ticks; otherwise it
        /// will not. (Default: true)
        /// </summary>
        /// <remarks>
        /// It is recommended to always set this to true, which the default. 
        /// This is due to performance advantages as well as an MvvmCross 
        /// documented issue:
        /// https://github.com/slodge/MvvmCross/issues/213#issuecomment-24610834
        /// </remarks>
        public bool StoreDateTimeAsTicks { get; set; }        
        /// <summary>
        /// States which type of database should be created. (Default: File)
        /// </summary>
        public DatabaseType Type { get; set; }

        /// <summary>
        /// Types of databases that can be created.
        /// </summary>
        public enum DatabaseType 
        {
            /// <summary>
            /// Specifies a file based database. (Default)
            /// </summary>
            File,
            /// <summary>
            /// Specifies a pure in-memory database.
            /// </summary>
            InMemory,
            /// <summary>
            /// Specifies a temporary file based database.
            /// </summary>
            Temporary,
        }
    }

    public interface ISQLiteConnectionFactoryEx
    {
        /// <summary>
        /// Creates a SQLite database using the provided options.
        /// </summary>
        /// <param name="options">The options to use to create a SQLite connection.</param>
        /// <returns>A ISQLiteConnection.</returns>
        ISQLiteConnection CreateEx(SQLiteConnectionOptions options);
        /// <summary>
        /// Creates a SQLite database using the provided path2 and options.
        /// </summary>
        /// <param name="address">Address for the database.</param>
        /// <remarks>The provide path2 will override any provided in the options.</remarks>
        /// <param name="options">The options to use to create a SQLite connection.</param>
        /// <returns>A ISQLiteConnection.</returns>
        ISQLiteConnection CreateEx(string address, SQLiteConnectionOptions options = null);
    }

    public interface ISQLiteConnectionFactory
    {
        /// <summary>
        /// Creates a SQLite database using the provided path2.
        /// </summary>
        /// <param name="address">Address for the database.</param>
        /// <returns>A ISQLiteConnection.</returns>
        ISQLiteConnection Create(string address);
        /// <summary>
        /// Creates a in-memory SQLite database.
        /// </summary>
        /// <remarks>
        /// This method will create NO FILES on disk and it will be a new 
        /// database that exists only in memory. Once the connection is closed
        /// the database will no longer exist. You can open multiple in memory
        /// databases and they will each be created as an isolated in-memory 
        /// databases.
        /// </remarks>
        /// <returns>An ISQLiteConnection to a isolated in-memory database.</returns>
        ISQLiteConnection CreateInMemory();
        /// <summary>
        /// Creates a new temporary SQLite database.
        /// </summary>
        /// <remarks>
        /// Each call will create a new temporary file based database. Each connection 
        /// will therefore be to is own temporary database. Once the connection that 
        /// created it is closed the temporary database will be deleted.
        /// <para>
        /// Please note that while a file is created for each temporary database
        /// that typically it will reside in the in-memory pager cache. This is
        /// still different from a pure in-memory database created by the Created*InMemory
        /// methods in that the temporary database may be flushed to the file if
        /// it becomes to large or if the underlying SQLite engine is placed under 
        /// memory pressure. This never occurs with the pure in-memory database options.
        /// </para>
        /// </remarks>
        /// <returns>A ISQLiteConnection to a temporary database.</returns>
        ISQLiteConnection CreateTemp();
    }

    [Flags]
    public enum CreateFlags
    {
        None = 0,
        ImplicitPK = 1,    // create a primary key for field called 'Id' (Orm.ImplicitPkName)
        ImplicitIndex = 2, // create an index for fields ending in 'Id' (Orm.ImplicitIndexSuffix)
        AllImplicit = 3,   // do both above

        AutoIncPK = 4      // force PK field to be auto inc
    }

    public class ColumnInfo
    {
        [Column("name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class CreateTablesResult
    {
        public Dictionary<Type, int> Results { get; private set; }

        public CreateTablesResult()
        {
            this.Results = new Dictionary<Type, int>();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedAttribute : Attribute
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public virtual bool Unique { get; set; }

        public IndexedAttribute()
        {
        }

        public IndexedAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : IndexedAttribute
    {
        public override bool Unique
        {
            get { return true; }
            set { /* throw?  */ }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; private set; }

        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CollationAttribute : Attribute
    {
        public string Value { get; private set; }

        public CollationAttribute(string collation)
        {
            Value = collation;
        }
    }

    public interface ITableMapping
    {
        string TableName { get; }
    }

    public interface ITableQuery<T> : IEnumerable<T> where T : new()
    {
        ISQLiteConnection Connection { get; }

        ITableQuery<T> Where(Expression<Func<T, bool>> predExpr);

        ITableQuery<T> Take(int n);

        ITableQuery<T> Skip(int n);

        T ElementAt(int index);

        ITableQuery<T> Deferred();

        ITableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr);

        ITableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr);

        ITableQuery<TResult> Join<TInner, TKey, TResult>(
            ITableQuery<TInner> inner,
            Expression<Func<T, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<T, TInner, TResult>> resultSelector)
            where TResult : new()
            where TInner : new();

        ITableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : new();

        int Count();

        int Count(Expression<Func<T, bool>> predExpr);

        IEnumerator<T> GetEnumerator();

        T First();

        T FirstOrDefault();
    }

    public interface ISQLiteCommand
    {
        string CommandText { get; set; }

        int ExecuteNonQuery();

        IEnumerable<T> ExecuteDeferredQuery<T>();

        List<T> ExecuteQuery<T>();

        List<T> ExecuteQuery<T>(ITableMapping map);

        IEnumerable<T> ExecuteDeferredQuery<T>(ITableMapping map);

        SQLiteDataResultSet ExecuteDeferredQuery();

        T ExecuteScalar<T>();

        void Bind(string name, object val);

        void Bind(object val);
    }

    /// <summary>
    /// Represents an open connection to a SQLite database.
    /// </summary>
    public interface ISQLiteConnection : IDisposable
    {
        string DatabasePath { get; }

        bool TimeExecution { get; set; }

        bool Trace { get; set; }

        bool StoreDateTimeAsTicks { get; }

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        TimeSpan BusyTimeout { get; set; }

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapping to the database is returned.
        /// </param>         
        /// <param name="createFlags">
        /// Optional flags allowing implicit PK and indexes based on naming conventions
        /// </param>     
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains 
        /// methods to set and get properties of objects.
        /// </returns>
        ITableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains 
        /// methods to set and get properties of objects.
        /// </returns>
        ITableMapping GetMapping<T>();

        /// <summary>
        /// Executes a "drop table" on the database.  This is non-recoverable.
        /// </summary>
        int DropTable<T>();

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        int CreateTable<T>(CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <param name="ty">Type to reflect to a database table.</param>
        /// <param name="createFlags">Optional flags allowing implicit PK and indexes based on naming conventions.</param>  
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Creates an index for the specified table and column.
        /// </summary>
        /// <param name="indexName">Name of the index to create</param>
        /// <param name="tableName">Name of the database table</param>
        /// <param name="columnName">Name of the column to index</param>
        /// <param name="unique">Whether the index should be unique</param>
        int CreateIndex(string indexName, string tableName, string columnName, bool unique = false);

        /// <summary>
        /// Creates an index for the specified table and column.
        /// </summary>
        /// <param name="tableName">Name of the database table</param>
        /// <param name="columnName">Name of the column to index</param>
        /// <param name="unique">Whether the index should be unique</param>
        int CreateIndex(string tableName, string columnName, bool unique = false);

        /// <summary>
        /// Creates an index for the specified object property.
        /// e.g. CreateIndex<Client>(c => c.Name);
        /// </summary>
        /// <typeparam name="T">Type to reflect to a database table.</typeparam>
        /// <param name="property">Property to index</param>
        /// <param name="unique">Whether the index should be unique</param>
        void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false);

        List<ColumnInfo> GetTableInfo(string tableName);

        /// <summary>
        /// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        /// in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the command text.
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        ISQLiteCommand CreateCommand(string cmdText, params object[] ps);

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        int Execute(string query, params object[] args);

        T ExecuteScalar<T>(string query, params object[] args);

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        List<T> Query<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// The enumerator will call sqlite3_step on each call to MoveNext, so the database
        /// connection must remain open for the lifetime of the enumerator.
        /// </returns>
        IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        List<object> Query(ITableMapping map, string query, params object[] args);

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// The enumerator will call sqlite3_step on each call to MoveNext, so the database
        /// connection must remain open for the lifetime of the enumerator.
        /// </returns>
        IEnumerable<object> DeferredQuery(ITableMapping map, string query, params object[] args);

        /// <summary>
        /// Execute a query to return a data result set of rows and columns
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        SQLiteDataResultSet QueryDataResults(string query, params object[] args);

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>
        ITableQuery<T> Table<T>() where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        T Get<T>(object pk) where T : new();

        /// <summary>
        /// Attempts to retrieve the first object that matches the predicate from the table
        /// associated with the specified type. 
        /// </summary>
        /// <param name="predicate">
        /// A predicate for which object to find.
        /// </param>
        /// <returns>
        /// The object that matches the given predicate. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        T Get<T>(Expression<Func<T, bool>> predicate) where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key or null
        /// if the object is not found.
        /// </returns>
        T Find<T>(object pk) where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <param name="map">
        /// The TableMapping used to identify the object type.
        /// </param>
        /// <returns>
        /// The object with the given primary key or null
        /// if the object is not found.
        /// </returns>
        object Find(object pk, ITableMapping map);

        /// <summary>
        /// Attempts to retrieve the first object that matches the predicate from the table
        /// associated with the specified type. 
        /// </summary>
        /// <param name="predicate">
        /// A predicate for which object to find.
        /// </param>
        /// <returns>
        /// The object that matches the given predicate or null
        /// if the object is not found.
        /// </returns>
        T Find<T>(Expression<Func<T, bool>> predicate) where T : new();

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        /// <example cref="System.InvalidOperationException">Throws if a transaction has already begun.</example>
        void BeginTransaction();

        /// <summary>
        /// Creates a savepoint in the database at the current point in the transaction timeline.
        /// Begins a new transaction if one is not in progress.
        /// 
        /// Call <see cref="RollbackTo"/> to undo transactions since the returned savepoint.
        /// Call <see cref="Release"/> to commit transactions after the savepoint returned here.
        /// Call <see cref="Commit"/> to end the transaction, committing all changes.
        /// </summary>
        /// <returns>A string naming the savepoint.</returns>
        string SaveTransactionPoint();

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/> or <see cref="SaveTransactionPoint"/>.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back the savepoint created by <see cref="BeginTransaction"/> or SaveTransactionPoint.
        /// </summary>
        /// <param name="savepoint">The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback"/></param>
        void RollbackTo(string savepoint);

        /// <summary>
        /// Releases a savepoint returned from <see cref="SaveTransactionPoint"/>.  Releasing a savepoint 
        ///    makes changes since that savepoint permanent if the savepoint began the transaction,
        ///    or otherwise the changes are permanent pending a call to <see cref="Commit"/>.
        /// 
        /// The RELEASE command is like a COMMIT for a SAVEPOINT.
        /// </summary>
        /// <param name="savepoint">The name of the savepoint to release.  The string should be the result of a call to <see cref="SaveTransactionPoint"/></param>
        void Release(string savepoint);

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        void Commit();

        /// <summary>
        /// Executes <param name="action"> within a (possibly nested) transaction by wrapping it in a SAVEPOINT. If an
        /// exception occurs the whole transaction is rolled back, not just the current savepoint. The exception
        /// is rethrown.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
        /// of operations on the connection but should never call <see cref="BeginTransaction"/> or
        /// <see cref="Commit"/>.
        /// </param>
        void RunInTransaction(Action action);

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int InsertAll(System.Collections.IEnumerable objects);

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int InsertAll(System.Collections.IEnumerable objects, string extra);

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int InsertAll(System.Collections.IEnumerable objects, Type objType);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// If a UNIQUE constraint violation occurs with
        /// some pre-existing object, this function deletes
        /// the old object.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows modified.
        /// </returns>
        int InsertOrReplace(object obj);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, Type objType);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// If a UNIQUE constraint violation occurs with
        /// some pre-existing object, this function deletes
        /// the old object.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows modified.
        /// </returns>
        int InsertOrReplace(object obj, Type objType);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, string extra);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, string extra, Type objType);

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        int Update(object obj);

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        int Update(object obj, Type objType);

        /// <summary>
        /// Updates all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// The number of rows modified.
        /// </returns>
        int UpdateAll(System.Collections.IEnumerable objects);

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="objectToDelete">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        int Delete(object objectToDelete);

        /// <summary>
        /// Deletes the object with the specified primary key.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key of the object to delete.
        /// </param>
        /// <returns>
        /// The number of objects deleted.
        /// </returns>
        /// <typeparam name='T'>
        /// The type of object.
        /// </typeparam>
        int Delete<T>(object primaryKey);

        /// <summary>
        /// Deletes all the objects from the specified table.
        /// WARNING WARNING: Let me repeat. It deletes ALL the objects from the
        /// specified table. Do you really want to do that?
        /// </summary>
        /// <returns>
        /// The number of objects deleted.
        /// </returns>
        /// <typeparam name='T'>
        /// The type of objects to delete.
        /// </typeparam>
        int DeleteAll<T>();

        void Close();
    }

    /// <summary>
    /// Base class that each platform specific SQLiteConnectionFactory 
    /// should extend.
    /// </summary>
    public abstract class MvxBaseSQLiteConnectionFactory
        : ISQLiteConnectionFactory
            , ISQLiteConnectionFactoryEx
    {
        private const string InMemoryDatabase = ":memory:";

        public virtual ISQLiteConnection Create(string address)
        {
            return CreateEx(address);
        }

        public virtual ISQLiteConnection CreateInMemory()
        {
            var options = new SQLiteConnectionOptions { Type = SQLiteConnectionOptions.DatabaseType.InMemory, };
            return CreateEx(options);
        }

        public virtual ISQLiteConnection CreateTemp()
        {
            var options = new SQLiteConnectionOptions { Type = SQLiteConnectionOptions.DatabaseType.Temporary, };
            return CreateEx(options);
        }

        public virtual ISQLiteConnection CreateEx(SQLiteConnectionOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException("options");
            switch (options.Type)
            {
                case SQLiteConnectionOptions.DatabaseType.InMemory: 
                    return CreateInMemoryDb(options);
                case SQLiteConnectionOptions.DatabaseType.Temporary: 
                    return CreateTempDb(options);
                default: 
                    return CreateFileDb(options);
            }
        }

        public virtual ISQLiteConnection CreateEx(string address, SQLiteConnectionOptions options = null)
        {
            options = options ?? new SQLiteConnectionOptions();
            options.Address = address;
            return CreateEx(options);
        }

        private ISQLiteConnection CreateFileDb(SQLiteConnectionOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Address))
                throw new ArgumentException(Properties.Resources.CreateFileDbInvalidAddress);
            var path = options.BasePath ?? GetDefaultBasePath();
            string filePath = LocalPathCombine(path, options.Address);
            return CreateSQLiteConnection(filePath, options.StoreDateTimeAsTicks);
        }

        private ISQLiteConnection CreateInMemoryDb(SQLiteConnectionOptions options)
        {
            return CreateSQLiteConnection(InMemoryDatabase, options.StoreDateTimeAsTicks);
        }

        private ISQLiteConnection CreateTempDb(SQLiteConnectionOptions options)
        {
            return CreateSQLiteConnection(string.Empty, options.StoreDateTimeAsTicks);
        }

        /// <summary>
        /// Returns the platform specific default base path1.
        /// </summary>
        /// <returns>
        /// Returns default base path.
        /// </returns>
        protected abstract string GetDefaultBasePath();
        /// <summary>
        /// Combines two strings into a platform specific path1.
        /// </summary>
        /// <remarks>
        /// If one of the specified paths is a zero-length string, 
        /// this method returns the other path. If <paramref name="path2"/> 
        /// contains an absolute path, this method returns 
        /// <paramref name="path2"/>.
        /// </remarks>
        /// <param name="path1">The first path1 to combine</param>
        /// <param name="path2">The second path1 to combine.</param>
        /// <returns>
        /// The combined paths.
        /// </returns>
        protected abstract string LocalPathCombine(string path1, string path2);
        /// <summary>
        /// Creates the platform specific SQLiteConnection.
        /// </summary>
        /// <param name="databasePath">
        /// The name of a file that does or will contain the database.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// If true will store DateTime properties as ticks; otherwise it
        /// will not.
        /// </param>
        /// <returns>Returns the interface to a SQLiteConnection.</returns>
        protected abstract ISQLiteConnection CreateSQLiteConnection(string databasePath, bool storeDateTimeAsTicks);
    }
}