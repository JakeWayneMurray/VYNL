using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using BespokeFusion;
using MongoDB.Driver;

namespace VinylCollectionApplication.FetchInfo
{
    static class VerifiedAccounts
    {
       public static List<Account> accounts = new List<Account>();

        static public async Task getAccountsAsync()
        {
            var bsonCollection = getCollection();

            using (IAsyncCursor<BsonDocument> cursor = await bsonCollection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        accounts.Add(BsonSerializer.Deserialize<Account>(document));
                    }
                }
            }
        }

        private static IMongoCollection<BsonDocument> getCollection()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://DadsThrobber:BrianSucks@vynl.4q0um.mongodb.net/VynLDatabase?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("VynLDatabase");
            return database.GetCollection<BsonDocument>("VynLCollection");
        }


    }
}
