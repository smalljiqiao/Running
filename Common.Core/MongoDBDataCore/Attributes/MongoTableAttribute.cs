using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDBDataCore.Attributes
{

    [AttributeUsage(AttributeTargets.Class)]
   public class MongoTableAttribute: Attribute
    {
        public string Database { get; set; }

        public string TableName { get; set; }

        public MongoTableAttribute(string database,string tablename)
        {
            this.Database = database;
            this.TableName = tablename;
        }


    }
}
