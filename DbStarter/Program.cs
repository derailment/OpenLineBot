using Dapper;
using System;
using System.Data.SQLite;
using System.IO;

namespace DbStarter
{
    class Program
    {

        static string dbPath = @"../../../OpenLineBot/App_Data/LineBotDb.sqlite";
        static string cnStr = "data source=" + dbPath;

        static void Main(string[] args)
        {
            InitSQLiteDb();
            Console.Read();
        }

        static void InitSQLiteDb()
        {
            if (File.Exists(dbPath)) throw new Exception(dbPath + " already existed !");
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute(@"CREATE TABLE Conversation (Id INTEGER PRIMARY KEY AUTOINCREMENT, QuestionNumber INTEGER NOT NULL, Answer TEXT NOT NULL, LineUserId TEXT NOT NULL)");
            }

            Console.WriteLine("Successfully create table !");
        }

    }
}
