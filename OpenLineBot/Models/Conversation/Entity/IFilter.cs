namespace OpenLineBot.Models.Conversation.Entity
{
    public interface IFilter
    {
        bool Pass(string s);
    }
}
