using isRock.LineBot;
using OpenLineBot.Models.Conversation.Entity;
using OpenLineBot.Models.System;
using OpenLineBot.Service;
using System;
using System.Linq;

namespace OpenLineBot.Models.Conversation.Handler.Custom
{
    public class ByeByeHandler<T> : Handler<T> where T : IConversationEntity 
    {
        public ByeByeHandler(BotService bot) : base(bot) { }

        public override void HandleRequest(object[] objs = null)
        {
            DbService.Remove(Bot.UserInfo.userId);
            Bot.PushMessage("Bye bye");
        }
    }

    public class NextQuestionHandler<T> : Handler<T> where T : IConversationEntity
    {
        public NextQuestionHandler(BotService bot) : base(bot) { }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            int lastQuestionNumber = DbService.LastQuestionNumber(Bot.UserInfo.userId);

            DbService.Update(Bot.UserInfo.userId, lastQuestionNumber, text);
            DbService.AddRecord(Bot.UserInfo.userId, lastQuestionNumber + 1);

            // Extra question hint
            if (lastQuestionNumber + 1 == 4)
            {
                Bot.PushMessage("從" + DbService.QueryAnswer(Bot.UserInfo.userId, 3).Replace("T", " ") + "開始起算");
            }

            // Query answers before the final confirm question
            if (lastQuestionNumber + 1 == ConvEntity.MaxOrder) {
                Bot.PushMessage("你剛剛說...");
                for(var order = 1; order < ConvEntity.MaxOrder; order++)
                {
                    Bot.PushMessage(ConvEntity.GetQuestionTextOnly(order) + " " + DbService.QueryAnswer(Bot.UserInfo.userId, order));
                }
            }

            ConvEntity.PushQuestion(lastQuestionNumber + 1);

        }
    }

    public class FirstQuestionHandler<T> : Handler<T> where T : IConversationEntity
    {
        public FirstQuestionHandler(BotService bot) : base(bot) { }

        public override void HandleRequest(object[] objs = null)
        {
            DbService.AddRecord(Bot.UserInfo.userId, 1);
            ConvEntity.PushQuestion(1);
        }
    }

    public class ComplaintHandler<T> : Handler<T> where T : IConversationEntity
    {
        public ComplaintHandler(BotService bot) : base(bot) { }

        public override void HandleRequest(object[] objs = null)
        {
            int lastQuestionNumber = DbService.LastQuestionNumber(Bot.UserInfo.userId);
            ConvEntity.PushComplaint(lastQuestionNumber);
            ConvEntity.PushQuestion(lastQuestionNumber);
        }
    }

    public class NoKeyWordHandler<T> : Handler<T> where T : IConversationEntity
    {
        public NoKeyWordHandler(BotService bot) : base(bot) { }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            Bot.PushMessage("我聽不懂「" + text + "」，你要說「我要請假」。\n(離開請說「Bye bye」)");
        }
    }

    public class SubmitHandler<T> : Handler<T> where T : IConversationEntity
    {
        public SubmitHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("ByeBye", 0);
        }

        public override void HandleRequest(object[] objs = null)
        {
            Bot.PushMessage("填寫請假資料中...");

            // Request external web API...

            Bot.PushMessage("請假卡填寫完成。");

            // Call successor
            Successors[SuccessorDic["ByeBye"]].HandleRequest(null);
        }

    }

    public class FinalQuestionHandler<T> : Handler<T> where T : IConversationEntity
    {
        public FinalQuestionHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("Submit", 0);
            SuccessorDic.Add("ByeBye", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            // Call successor
            switch (text)
            {
                case "Y":
                    Successors[SuccessorDic["Submit"]].HandleRequest(null);
                    break;
                default:
                    Successors[SuccessorDic["ByeBye"]].HandleRequest(null);
                    break;
            }
        }

    }

    public class NotFinalQuestionHandler<T> : Handler<T> where T : IConversationEntity
    {
        public NotFinalQuestionHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("NextQuestion", 0);
            SuccessorDic.Add("Complaint", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            int lastQuestionNumber = DbService.LastQuestionNumber(Bot.UserInfo.userId);

            bool isPassed = ConvEntity.IsAnswerPassed(lastQuestionNumber, text);

            // Call successor
            switch (isPassed)
            {
                case true:
                    Successors[SuccessorDic["NextQuestion"]].HandleRequest(new object[] { text });
                    break;
                case false:
                    Successors[SuccessorDic["Complaint"]].HandleRequest(null);
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S009).Message);
            }
        }
    }

    public class RecordedHandler<T> : Handler<T> where T : IConversationEntity
    {
        public RecordedHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("FinalQuestion", 0);
            SuccessorDic.Add("NotFinalQuestion", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            bool isFinal = ConvEntity.MaxOrder == DbService.LastQuestionNumber(Bot.UserInfo.userId);

            // Call successor
            switch (isFinal)
            {
                case true:
                    Successors[SuccessorDic["FinalQuestion"]].HandleRequest(new object[] { text });
                    break;
                case false:
                    Successors[SuccessorDic["NotFinalQuestion"]].HandleRequest(new object[] { text });
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S009).Message);
            }
        }

    }

    public class NotRecordedHandler<T> : Handler<T> where T : IConversationEntity
    {
        public NotRecordedHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("FirstQuestion", 0);
            SuccessorDic.Add("NoKeyWord", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            bool hasKeyword = text.Contains("請假");

            // Call successor
            switch (hasKeyword)
            {
                case true:
                    Successors[SuccessorDic["FirstQuestion"]].HandleRequest(null);
                    break;
                case false:
                    Successors[SuccessorDic["NoKeyWord"]].HandleRequest(new object[] { text });
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S009).Message);
            }
        }

    }

    public class NotByeHandler<T> : Handler<T> where T : IConversationEntity
    {
        public NotByeHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("Recorded", 0);
            SuccessorDic.Add("NotRecorded", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            bool isRecorded = DbService.IsAny(Bot.UserInfo.userId);

            // Call successor
            switch (isRecorded)
            {
                case true:
                    Successors[SuccessorDic["Recorded"]].HandleRequest(new object[] { text });
                    break;
                case false:
                    Successors[SuccessorDic["NotRecorded"]].HandleRequest(new object[] { text });
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S009).Message);
            }
        }

    }

    public class TextHandler<T> : Handler<T> where T : IConversationEntity
    {
        public TextHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("ByeBye", 0);
            SuccessorDic.Add("NotBye", 1);
        }

        public override void HandleRequest(object[] objs)
        {
            if (objs == null || objs.First() == null) throw new Exception(new Error(ErrCode.S008).Message);

            string text = (string)objs.First();

            bool hasBye = text.ToLower().Contains("bye");

            // Call successor
            switch (hasBye)
            {
                case true:
                    Successors[SuccessorDic["ByeBye"]].HandleRequest(null);
                    break;
                case false:
                    Successors[SuccessorDic["NotBye"]].HandleRequest(new object[] { text });
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S009).Message);
            }
        }

    }

    public class LineEventHandler<T> : Handler<T> where T : IConversationEntity
    {
        public LineEventHandler(BotService bot) : base(bot) {
            SuccessorDic.Add("Text", 0);
        }

        public override void HandleRequest(object[] objs = null)
        {
            Event evt = Bot.LineEvent;

            // Call successor
            switch (evt.type)
            {
                case "message":
                    if (evt.message.type.Equals("text"))
                    {
                        Successors[SuccessorDic["Text"]].HandleRequest(new object[] { Bot.LineEvent.message.text });
                    }
                    else
                    {
                        throw new Exception(new Error(ErrCode.S010).Message);
                    }
                    break;
                case "postback":
                    if (Bot.LineEvent.postback.data.Equals("datetimepicker"))
                    {
                        Successors[SuccessorDic["Text"]].HandleRequest(new object[] { Bot.LineEvent.postback.Params.datetime });
                    }
                    else {
                        Successors[SuccessorDic["Text"]].HandleRequest(new object[] { Bot.LineEvent.postback.data});
                    }
                    break;
                default:
                    throw new Exception(new Error(ErrCode.S002).Message);
            }
        }

    }

}