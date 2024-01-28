using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlementEvaluator
{
    public static class DateTimeExtensions
    {
        public static DateTime LastDayOfWeek(this DateTime _date, DayOfWeek dayofweek)
        {
            return _date.AddDays(-1 * ((_date.DayOfWeek - dayofweek) % 7)).Date;
        }

        public static DateTime NextDayOfWeek(this DateTime _date, DayOfWeek dayofweek)
        {
            return _date.LastDayOfWeek(dayofweek).AddDays(7).Date;
        }
    }
    public class CarrierProfile
    {
        public string Id { get; set; }
        public DayOfWeek SettlementCutoffDayOfWeek { get; set; }
        public TimeOnly SettlementCutoffTime { get; set; }
        public int DaysToSettlementFromCutoff { get; set; }
        public string AccountNumber { get; set; }
        private DateTime CurrentCutoff
        {
            get { return DateTime.Now.NextDayOfWeek(SettlementCutoffDayOfWeek); }
        }

        public DateTime SettlmentDate(DateTime deliveryDate)
        {
            return deliveryDate.NextDayOfWeek(SettlementCutoffDayOfWeek).AddDays(DaysToSettlementFromCutoff);
        }
        public DateTime PreviousSettlementDate(DateTime deliveryDate)
        {
            return deliveryDate.LastDayOfWeek(SettlementCutoffDayOfWeek);
        }
    }
}
