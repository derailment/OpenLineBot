using OpenLineBot.Service;

namespace OpenLineBot.Models.Conversation.Entity.Custom
{
    public class LeaveApply : ConversationEntity
    {
        public LeaveApply(BotService bot) : base (bot) { }

        [Order(1)]
        [TextQuestion("給我員工編號?")]
        [Answer(typeof(EmployIdFilter), "是4個數字好嗎!")]
        public string EmployId { get; set; }

        [Order(2)]
        [TextPickerQuestion("請哪種假?", new string[] {"公假", "事假", "病假"})]
        [Answer(typeof(LeaveCateFilter), "用選的，不要自己亂回!")]
        public string LeaveCate { get; set; }

        [Order(3)]
        [DateTimePickerQuestion("何時開始?")]
        [Answer(typeof(LeaveStartFilter), "用選的，不要自己亂回!")]
        public string LeaveStart { get; set; }

        [Order(4)]
        [TextQuestion("請幾天?")]
        [Answer(typeof(LeaveDaysFilter), "給個數目好嗎!")]
        public string LeaveDays { get; set; }

        [Order(5)]
        [TextQuestion("請幾小時?")]
        [Answer(typeof(LeaveHoursdFilter), "0到8小時!")]
        public string LeaveHours { get; set; }
        
        [Order(6)]
        [ConfirmQuestion("要提交了嗎?")]
        [Answer(typeof(SubmitFilter), "用選的，不要自己亂回!")]
        public string Submit { get; set; }
        
    }
}