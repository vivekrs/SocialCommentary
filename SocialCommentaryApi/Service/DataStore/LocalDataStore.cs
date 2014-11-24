using System;
using System.Xml.Linq;

namespace SocialCommentaryApi.Service.DataStore
{
    public class LocalDataStore:IDataStore
    {
        public void Save(string id, DateTime createdOn, string stream, Type entityType, string entity)
        {
            using (var dc = new SocialDataStoreDataContext())
            {
                dc.SocialStreams.InsertOnSubmit(
                    new SocialStream
                    {
                        IdString = id,
                        CreatedOn = createdOn,
                        Stream = stream,
                        EntityType = entityType.Name,
                        Entity = entity
                    });

                dc.SubmitChanges();
            }
        }
    }

    public interface IDataStore
    {
        void Save(string id, DateTime createdOn, string stream, Type entityType, string entity);
    }
}