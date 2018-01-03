using OpenLineBot.Models.System;
using System;

namespace OpenLineBot.Models.Conversation.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class Answer : Attribute
    {
        Type _Filter = null;
        string _Complaint = "";

        public Answer(Type f, string c)
        {
            if (f.GetInterface("IFilter") == null) throw new Exception(new Error(ErrCode.S006).Message);
            _Filter = f;
            _Complaint = c;
        }

        public Type Filter
        {
            get
            {
                return _Filter;
            }
        }

        public string Complaint
        {
            get
            {
                return _Complaint;
            }
        }
    }
}