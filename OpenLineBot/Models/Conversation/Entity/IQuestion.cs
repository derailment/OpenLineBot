namespace OpenLineBot.Models.Conversation.Entity
{
    public interface IQuestion
    {
        BotPushType PushType { get; }
        string Question { get; }
    }

    public enum BotPushType { Text, TextPicker, DataTimePicker, Confirm }

}