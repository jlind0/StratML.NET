using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using StratML.Data.Core;


namespace StratML.Data
{
    public abstract class CosmosDataAdapter
    {
        protected CosmosDataToken Token { get; private set; }
        public Uri DataPath
        {
            get { return UriFactory.CreateDocumentCollectionUri(Token.Database, Token.Collection); }
        }
        public CosmosDataAdapter(CosmosDataToken token)
        {
            this.Token = token;
        }
        protected DocumentClient OpenClient()
        {
            return new DocumentClient(Token.Path, Token.Key);
        }
        protected async Task UseClient(Func<DocumentClient, Task> clientFunc)
        {
            using (var client = OpenClient())
            {
                await clientFunc(client);
            }
        }
        protected IOrderedQueryable<T> CreateQuery<T>(DocumentClient client, FeedOptions options = null)
        {
            return client.CreateDocumentQuery<T>(this.DataPath, options);
        }
    }
}
