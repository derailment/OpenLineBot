using Dapper;
using OpenLineBot.Models.System;
using OpenLineBot.Service;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace OpenLineBot.Repository
{
    public class ConversationRepository
    {
        string CnStr = "";
        BotService Bot;

        public ConversationRepository(BotService bot)
        {
            string dbPath = HttpContext.Current.Server.MapPath(@"../App_Data/LineBotDb.sqlite");
            CnStr = "data source=" + dbPath;
            Bot = bot;
        }

        public void AddRecord(string userId, int questionNumber)
        {
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    cn.Execute(@"INSERT INTO Conversation(LineUserId, QuestionNumber, Answer) VALUES (@userId, @questionNumber, @answer)", new { @userId = userId, @questionNumber = questionNumber, @answer = ""});
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
        }

        public void Remove(string userId)
        {
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    cn.Execute(@"DELETE FROM Conversation WHERE LineUserId=@userId", new { @userId = userId });
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
        }

        public void Update(string userId, int questionNumber, string answer)
        {
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    cn.Execute(@"UPDATE Conversation SET Answer=@answer WHERE LineUserId=@userId AND QuestionNumber=@questionNumber", new { @userId = userId, @questionNumber = questionNumber, @answer = answer });
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
        }

        public bool IsAny(string userId)
        {
            bool ret = false;
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    ret = cn.Query(@"SELECT * FROM Conversation WHERE LineUserId=@userId", new { @userId = userId }).Any();
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
            return ret;
        }

        public int LastQuestionNumber(string userId)
        {
            int ret = 0;
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    ret = cn.Query<int>(@"SELECT COALESCE(MAX(QuestionNumber), 0) FROM Conversation WHERE LineUserId=@userId", new { @userId = userId }).First();
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
            return ret;
        }

        public bool HasAnswer(string userId, int questionNumber)
        {
            bool ret = false;
            using (var cn = new SQLiteConnection(CnStr))
            {
                try
                {
                    ret = !string.IsNullOrEmpty(cn.Query<string>(@"SELECT Answer FROM Conversation WHERE LineUserId=@userId AND QuestionNumber=@questionNumber", new { @userId = userId, @questionNumber = questionNumber }).First());
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
            return ret;
        }

        public string QueryAnswer(string userId, int questionNumber)
        {
            string answer = null;
            using (var cn = new SQLiteConnection(CnStr))
            { 
                try
                {
                    answer = cn.Query<string>(@"SELECT Answer FROM Conversation WHERE LineUserId=@userId AND QuestionNumber=@questionNumber", new { @userId = userId, @questionNumber = questionNumber }).First();
                }
                catch (Exception ex)
                {
                    Bot.Notify(new Exception(new Error(ErrCode.D001, Bot.UserInfo.userId, ex.Message).Message));
                }
            }
            return answer;
        }
    }
}