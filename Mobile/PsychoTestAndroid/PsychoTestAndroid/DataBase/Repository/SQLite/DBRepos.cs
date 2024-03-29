﻿using Android.App;
using PsychoTestAndroid.DataBase.Entity;
using PsychoTestAndroid.DataBase.Interfaces;
using SQLite;
using System.IO;

namespace PsychoTestAndroid.DataBase.Repository.SQLite
{
    public class DBRepos : IDbRepos
    {
        SQLiteConnection db;
        TestRepos test;
        ScaleRepos scale;
        TestCalcInfoRepos testCalcInfo;

        public DBRepos()
        {
            db = new SQLiteConnection(GetPath());
            db.CreateTable<DbTest>();
            db.CreateTable<DbScale>();
            db.CreateTable<DbTestCalcInfo>();
        }

        string GetPath()
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                Application.Context.GetString(Resource.String.app_name));
        }

        public IRepository<DbTest> Test
        {
            get
            {
                if (test == null)
                    test = new TestRepos(db);
                return test;
            }
        }

        public IRepository<DbScale> Scale
        {
            get
            {
                if (scale == null)
                    scale = new ScaleRepos(db);
                return scale;
            }
        }

        public IRepository<DbTestCalcInfo> TestCalcInfo
        {
            get
            {
                if (testCalcInfo == null)
                    testCalcInfo = new TestCalcInfoRepos(db);
                return testCalcInfo;
            }
        }

        public void DeleteAll()
        {
            db.DeleteAll<DbTest>();
        }
    }
}