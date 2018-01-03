namespace OpenLineBot.Models.Conversation.Entity
{
    public interface IConversationEntity {
        int MaxOrder { get; }
        void PushQuestion(int order);
        void PushComplaint(int order);
        bool IsAnswerPassed(int order, string text);
        string GetQuestionTextOnly(int order);
    }
}
