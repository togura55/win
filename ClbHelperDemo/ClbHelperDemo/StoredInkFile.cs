using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClbHelperDemo
{
    public class StoredInkFile
    {
        public List<Property> properties;
        public int count;

        public StoredInkFile()
        {
            properties = new List<Property>();
            count = 0;
        }

        public bool IsExisted(Microsoft.OneDrive.Sdk.Item obj)
        {
            bool res = false;  // no

            for (int i = 0; i< properties.Count; i++)
            {
                if (properties[i].Id == obj.Id)
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        public void Add(Microsoft.OneDrive.Sdk.Item obj)
        {
            Property item = new Property
            {
                Id = obj.Id,
                Name = obj.Name,
                Size = (long)obj.Size,
                CTag = obj.CTag,
                ETag = obj.ETag,
                CreatedDateTime = (DateTimeOffset)obj.CreatedDateTime
            };

            properties.Add(item);
            count++;
        }

        public class Property
        {
            public System.DateTimeOffset CreatedDateTime;
            public string Id;
            public string Name;
            public long Size;
            public string CTag;
            public string ETag;
            public readonly System.DateTimeOffset ReadDateTime;

            public Property()
            {
                // set the current date/time
                ReadDateTime = System.DateTimeOffset.Now;

            }
        }
    }
}
