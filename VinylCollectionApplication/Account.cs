using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using BespokeFusion;


namespace VinylCollectionApplication
{
    public class Account
    {
        public ObjectId _id { get; set; }
        public string email { get; set; }
        public string userName {get;set;}
        public List<Friend> friends { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Date { get; set; }
        public bool isPrivate { get; set; }
        public List<Vinyl> Collection { get; set; }
        public bool isPremium { get; set; }


        public Account()
        {
            Date = DateTime.Today.ToString();
            Collection = new List<Vinyl>();
            isPrivate = false;
        }

        public Account(string fName, string lName, string userEmail, string username, bool isprivate, string pword)
        {
            userName = username;
            firstName = fName;
            lastName = lName;
            email = userEmail;
            password = pword;
            isPrivate = isprivate;
            friends = new List<Friend>();
            Date = DateTime.Today.ToString();
            Collection = new List<Vinyl>();
            isPremium = false;

        }
        public void Add(Vinyl v)
        {
            Collection.Add(v);
        }

        private static IMongoCollection<BsonDocument> getCollection()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://DadsThrobber:BrianSucks@vynl.4q0um.mongodb.net/VynLDatabase?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("VynLDatabase");
            return database.GetCollection<BsonDocument>("VynLCollection");
        }

        public void upload()
        {
            try
            {
                var bsonCollection = getCollection();

                //deleting single record
                var Deleteone = bsonCollection.DeleteOneAsync(
                    Builders<BsonDocument>.Filter.Eq("_id", this._id));

                //inserting the new record
                bsonCollection.InsertOneAsync(this.ToBsonDocument());


            }
            catch (NullReferenceException)
            {
                MaterialMessageBox.Show("Nothing to save");
            }
        }

        public void delete()
        {
            try
            {
                var bsonCollection = getCollection();

                //deleting single record
                var Deleteone = bsonCollection.DeleteOneAsync(
                    Builders<BsonDocument>.Filter.Eq("_id", this._id));
                    
            }
            catch (NullReferenceException)
            {
                MaterialMessageBox.Show("Nothing to save");
            }
        }

        public bool isAbleToAdd()
        {
            if (!isPremium && Collection.Count >= 25)
                return false;
            else
                return true;
        }
    }
}