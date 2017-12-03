using System;
using System.Collections.Generic;
using System.Text;

namespace StratML.Data.Core
{
    public class CosmosDataToken
    {
        public Uri Path { get; private set; }
        public string Key { get; private set; }
        public string Database { get; private set; }
        public string Collection { get; private set; }
        public CosmosDataToken(Uri path, string key, string database, string collection)
        {
            Path = path;
            Key = key;
            Database = database;
            Collection = collection;
        }
    }
}
