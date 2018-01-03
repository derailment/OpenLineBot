using System.Linq;

namespace OpenLineBot.Models.System
{
    public enum ErrCode
    {
        S001, // Source is not from user
        S002, // Not handle this event type
        S003, // Question order is less than zero
        S004, // Question push type has not be defined
        S005, // Question order range exceeds
        S006, // Filter class must implement IFilter
        S007, // ConfirmQuestion attribute must be on top of the last property of concrete class of IConversationEntity  
        S008, // Argument or its first element should not be null
        S009, // Type error
        S010, // Not handle this message type
        S011, // Did not find out this question
        S012, // Successor setting error
        N001, // Not expected user behavior
        D001  // Database error
    }

    public class Error
    {

        public string Message { get; }
        public Error(ErrCode code, string userId = null, string detail = null){
            char type = code.ToString().First();
            switch (type)
            {
                case 'S':
                    Message = "[" + code.ToString() + "系統錯誤] 請通知管理員<ytw@mail.sinotech.com.tw>";
                    break;
                case 'N':
                    Message = "[" + code.ToString() + "系統正常] " + detail + "\n來自: " + userId;
                    break;
                case 'D':
                    Message = "[" + code.ToString() + "資料庫異常] " + detail + "\n來自: " + userId;
                    break;
                default:
                    break;
            }
        }
    }

}