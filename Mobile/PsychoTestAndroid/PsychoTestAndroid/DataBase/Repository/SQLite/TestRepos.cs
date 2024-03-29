﻿using PsychoTestAndroid.DataBase.Entity;
using PsychoTestAndroid.DataBase.Interfaces;
using System.Collections.Generic;
using SQLite;

namespace PsychoTestAndroid.DataBase.Repository.SQLite
{
    public class TestRepos : IRepository<DbTest>
    {
        SQLiteConnection db;

        public TestRepos(SQLiteConnection db)
        {
            this.db = db;
        }

        public List<DbTest> GetAll()
        {
            return db.Table<DbTest>().ToList();
        }

        public DbTest GetItem(int id)
        {
            return db.Get<DbTest>(id);
        }

        public void Create(DbTest item)
        {
            db.Insert(item);
        }

        public void Update(DbTest item)
        {
            db.Update(item);
        }

        public void Delete(DbTest item)
        {
            db.Delete(item);
        }

        public int Save()
        {
            return 1;
        }
    }
}