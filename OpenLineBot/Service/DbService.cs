using OpenLineBot.Repository;

namespace OpenLineBot.Service
{
    public class DatabaseService
    {
        ConversationRepository Repos;

        public DatabaseService(BotService bot)
        {
            Repos = new ConversationRepository(bot);
        }

        public void AddRecord(string userId, int questionNumber)
        {
            Repos.AddRecord(userId, questionNumber);
        }

        public void Remove(string userId)
        {
            Repos.Remove(userId);
        }

        public void Update(string userId, int questionNumber, string answer)
        {
            Repos.Update(userId, questionNumber, answer);
        }

        public bool IsAny(string userId)
        {
            return Repos.IsAny(userId);
        }

        public int LastQuestionNumber(string userId)
        {
            return Repos.LastQuestionNumber(userId);
        }

        public bool HasAnswer(string userId, int questionNumber)
        {
            return Repos.HasAnswer(userId, questionNumber);
        }

        public string QueryAnswer(string userId, int questionNumber) 
        {
            return Repos.QueryAnswer(userId, questionNumber);
        }
    }
}