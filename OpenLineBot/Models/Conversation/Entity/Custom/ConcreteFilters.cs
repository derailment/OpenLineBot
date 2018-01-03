using System;
using System.Text.RegularExpressions;

namespace OpenLineBot.Models.Conversation.Entity.Custom
{
    public class EmployIdFilter : IFilter
    {
        public bool Pass(string s)
        {
            Regex rgx = new Regex(@"^\d{4}$");
            return rgx.IsMatch(s);
        }
    }

    public class LeaveCateFilter : IFilter
    {
        public bool Pass(string s)
        {
            Regex rgx = new Regex(@"^(?:公假|事假|病假)$");
            return rgx.IsMatch(s);
        }
    }

    public class LeaveStartFilter : IFilter
    {
        public bool Pass(string s)
        {
            DateTime dateValue;
            return DateTime.TryParse(s, out dateValue);
        }
    }

    public class LeaveDaysFilter : IFilter
    {
        public bool Pass(string s)
        {
            Regex rgx = new Regex(@"^\d{1,3}$");
            return rgx.IsMatch(s);
        }
    }

    public class LeaveHoursdFilter : IFilter
    {
        public bool Pass(string s)
        {
            Regex rgx = new Regex(@"^[0-8]{1}$");
            return rgx.IsMatch(s);
        }
    }

    public class SubmitFilter : IFilter
    {
        public bool Pass(string s)
        {
            Regex rgx = new Regex(@"^(?:Y|N)$");
            return rgx.IsMatch(s);
        }
    }

}