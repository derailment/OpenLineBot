using OpenLineBot.Models.System;
using OpenLineBot.Service;
using System;
using System.Linq;

namespace OpenLineBot.Models.Conversation.Entity
{
    abstract public class ConversationEntity : IConversationEntity
    {
        int _MaxOrder = 0;
        BotService _Bot = null;

        public ConversationEntity(BotService bot)
        {
            _Bot = bot;
            if (!HasLastConfirm()) throw new Exception(new Error(ErrCode.S007).Message);
        }

        public int MaxOrder
        {
            get 
            {
                var props = GetType().GetProperties();
                
                foreach (var prop in props)
                {
                    var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                    if(orderAttr != null)
                    {
                        var id = orderAttr.Id;
                        if (id > _MaxOrder)
                        {
                            _MaxOrder = id;
                        }
                    }
                }
                return _MaxOrder;
            }
        }

        public void PushQuestion(int order)
        {
            if (order > MaxOrder || order <= 0) throw new Exception(new Error(ErrCode.S005).Message);

            var props = GetType().GetProperties();
            foreach(var prop in props)
            {
                var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                if (orderAttr.Id == order)
                {
                    var questionAttr = ((IQuestion[])prop.GetCustomAttributes(typeof(IQuestion), false)).FirstOrDefault();
                    switch (questionAttr.PushType) {
                        case BotPushType.Text:
                            _Bot.PushMessage(((TextQuestion)questionAttr).TextResponse);
                            break;
                        case BotPushType.TextPicker:
                            _Bot.PushMessage(((TextPickerQuestion)questionAttr).ButtonTemplateResponse);
                            break;
                        case BotPushType.DataTimePicker:
                            _Bot.PushMessage(((DateTimePickerQuestion)questionAttr).ButtonTemplateResponse);
                            break;
                        case BotPushType.Confirm:
                            _Bot.PushMessage(((ConfirmQuestion)questionAttr).ConfirmTemplateResponse);
                            break;
                        default:
                            throw new Exception(new Error(ErrCode.S004).Message);
                    }
                    break; 
                }                                
            }
        }

        public string GetQuestionTextOnly(int order)
        {
            if (order > MaxOrder || order <= 0) throw new Exception(new Error(ErrCode.S005).Message);

            var props = GetType().GetProperties();
            foreach (var prop in props)
            {
                var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                if (orderAttr.Id == order)
                {
                    var questionAttr = ((IQuestion[])prop.GetCustomAttributes(typeof(IQuestion), false)).FirstOrDefault();
                    return questionAttr.Question;
                }
            }

            throw new Exception(new Error(ErrCode.S011).Message);
        }

        public bool IsAnswerPassed(int order, string text)
        {
            if (order > MaxOrder || order <= 0) throw new Exception(new Error(ErrCode.S005).Message);

            bool pass = false;

            var props = GetType().GetProperties();
            foreach (var prop in props)
            {
                var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                if (orderAttr.Id == order)
                {
                    var answerAttr = ((Answer[])prop.GetCustomAttributes(typeof(Answer), false)).FirstOrDefault();
                    var filterType = answerAttr.Filter;
                    var filterObject = filterType.GetConstructors()[0].Invoke(null);
                    pass = (bool)filterType.GetMethod("Pass").Invoke(filterObject, new object[] { text });
                    break;
                }
            }

            return pass;
        }

        public void PushComplaint(int order)
        {
            if (order > MaxOrder || order <= 0) throw new Exception(new Error(ErrCode.S005).Message);

            var props = GetType().GetProperties();
            foreach (var prop in props)
            {
                var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                if (orderAttr.Id == order)
                {
                    var answerAttr = ((Answer[])prop.GetCustomAttributes(typeof(Answer), false)).FirstOrDefault();
                    var complaint = answerAttr.Complaint;
                    _Bot.PushMessage(complaint);
                    break;
                }
            }
        }

        public bool HasLastConfirm()
        {
            var props = GetType().GetProperties();
            foreach (var prop in props)
            {
                var orderAttr = ((Order[])prop.GetCustomAttributes(typeof(Order), false)).FirstOrDefault();
                if (orderAttr.Id == MaxOrder)
                {
                    var confirmQuestionAttr = ((ConfirmQuestion[])prop.GetCustomAttributes(typeof(ConfirmQuestion), false)).FirstOrDefault();
                    if (confirmQuestionAttr == null) throw new Exception(new Error(ErrCode.S007).Message);
                    else return true;
                }
            }
            return false;
        }

    }
}