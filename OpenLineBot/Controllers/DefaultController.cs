using OpenLineBot.Models.Conversation.Entity.Custom;
using OpenLineBot.Models.Conversation.Handler.Custom;
using OpenLineBot.Service;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace OpenLineBot.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpPost]
        public IHttpActionResult POST()
        {

            BotService bot = null;

            try
            {
                string postData = Request.Content.ReadAsStringAsync().Result;
                var receivedMessage = isRock.LineBot.Utility.Parsing(postData);
                var evt = receivedMessage.events.FirstOrDefault();

                SecretInfo secret = Load();
                bot = new BotService(secret.ChannelAccessToken, secret.AdminId, evt);

                var handlerSubmit = new SubmitHandler<LeaveApply>(bot);
                var handlerByeBye = new ByeByeHandler<LeaveApply>(bot);
                var handlerNoKeyWord = new NoKeyWordHandler<LeaveApply>(bot);
                var handlerComplaint = new ComplaintHandler<LeaveApply>(bot);
                var handlerFirstQuestion = new FirstQuestionHandler<LeaveApply>(bot);
                var handlerNextQuestion = new NextQuestionHandler<LeaveApply>(bot);
                var handlerFinalQuestion = new FinalQuestionHandler<LeaveApply>(bot);
                var handlerNotFinalQuestion = new NotFinalQuestionHandler<LeaveApply>(bot);
                var handlerRecorded = new RecordedHandler<LeaveApply>(bot);
                var handlerNotRecorded = new NotRecordedHandler<LeaveApply>(bot);
                var handlerNotBye = new NotByeHandler<LeaveApply>(bot);
                var handlerText = new TextHandler<LeaveApply>(bot);
                var handlerLineEvent = new LineEventHandler<LeaveApply>(bot);

                // Set seccessors
                handlerSubmit.SetSeccessor(handlerSubmit.SuccessorDic["ByeBye"], handlerByeBye);

                handlerFinalQuestion.SetSeccessor(handlerFinalQuestion.SuccessorDic["Submit"], handlerSubmit);
                handlerFinalQuestion.SetSeccessor(handlerFinalQuestion.SuccessorDic["ByeBye"], handlerByeBye);

                handlerNotFinalQuestion.SetSeccessor(handlerNotFinalQuestion.SuccessorDic["NextQuestion"], handlerNextQuestion);
                handlerNotFinalQuestion.SetSeccessor(handlerNotFinalQuestion.SuccessorDic["Complaint"], handlerComplaint);

                handlerRecorded.SetSeccessor(handlerRecorded.SuccessorDic["FinalQuestion"], handlerFinalQuestion);
                handlerRecorded.SetSeccessor(handlerRecorded.SuccessorDic["NotFinalQuestion"], handlerNotFinalQuestion);

                handlerNotRecorded.SetSeccessor(handlerNotRecorded.SuccessorDic["FirstQuestion"], handlerFirstQuestion);
                handlerNotRecorded.SetSeccessor(handlerNotRecorded.SuccessorDic["NoKeyWord"], handlerNoKeyWord);

                handlerNotBye.SetSeccessor(handlerNotBye.SuccessorDic["Recorded"], handlerRecorded);
                handlerNotBye.SetSeccessor(handlerNotBye.SuccessorDic["NotRecorded"], handlerNotRecorded);

                handlerText.SetSeccessor(handlerText.SuccessorDic["ByeBye"], handlerByeBye);
                handlerText.SetSeccessor(handlerText.SuccessorDic["NotBye"], handlerNotBye);

                handlerLineEvent.SetSeccessor(handlerLineEvent.SuccessorDic["Text"], handlerText);

                handlerLineEvent.HandleRequest();

                return Ok();
            }
            catch (Exception ex)
            {
                bot.Notify(ex);
                return Ok();
            }
        }

        SecretInfo Load()
        {
            string tokenPath = HttpContext.Current.Server.MapPath(@"../App_Data/secret.token");
            using (StreamReader r = new StreamReader(tokenPath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<SecretInfo>(json);
            }
        }

    }

    internal class SecretInfo
    {
        public string AdminId { get; set; }
        public string ChannelAccessToken { get; set; }
    }

}
