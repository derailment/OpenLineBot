using OpenLineBot.Models.System;
using System;

namespace OpenLineBot.Models.Conversation.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class Order : Attribute
    {
        int _Id = 0;

        public Order(int id)
        {
            if (id <= 0) throw new Exception(new Error(ErrCode.S003).Message);
            _Id = id;
        }

        public int Id {
            get
            {
                return _Id;
            }
        } 
    }
}