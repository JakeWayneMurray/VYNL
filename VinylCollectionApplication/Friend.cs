using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinylCollectionApplication
{
    public class Friend
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public List<Vinyl> Collection { get; set; }

        public Friend(string fName, string lName, string e, List<Vinyl> collection)
        {
            firstName = fName;
            lastName = lName;
            email = e;
            Collection = collection;
        }
    }
}
