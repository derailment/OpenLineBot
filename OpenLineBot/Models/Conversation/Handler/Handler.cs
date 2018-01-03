using OpenLineBot.Models.Conversation.Entity;
using OpenLineBot.Models.System;
using OpenLineBot.Service;
using System;
using System.Collections.Generic;

namespace OpenLineBot.Models.Conversation.Handler
{
    abstract public class Handler<T> where T : IConversationEntity
    {
        protected List<Handler<T>> Successors;
        protected BotService Bot;
        protected IConversationEntity ConvEntity;
        protected DatabaseService DbService;
        public Dictionary<string, int> SuccessorDic { get; }

        public Handler (BotService bot) {

            Successors = new List<Handler<T>>();
            Bot = bot;
            ConvEntity = (T)Activator.CreateInstance(typeof(T), new object[] { Bot });
            DbService = new DatabaseService(Bot);
            SuccessorDic = new Dictionary<string, int>();
        }

        public void SetSeccessor(int index, Handler<T> handler)
        {
            foreach(var successor in SuccessorDic)
            {
                if (successor.Value == index) {
                    string branchName = successor.Key + "Handler";
                    string handlerName = handler.GetType().Name.TrimEnd(new Char[] { '`', '1'});
                    if (!handlerName.Equals(branchName)) throw new Exception(new Error(ErrCode.S012).Message);
                    break;
                }
            }

            Successors.Insert(index, handler);
        }

        abstract public void HandleRequest(object[] objs) ;
    }

}