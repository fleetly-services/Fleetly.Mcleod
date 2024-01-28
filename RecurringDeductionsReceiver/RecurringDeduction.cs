using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecurringDeductionsReceiver
{
    public  class RecurringDeduction
    {
        public string id { get; set; }
        public decimal amount { get; set; }
        public string cycle_id { get; set; }
        public string deduct_code_id { get; set; }
        public DateTime deduct_end_date { get; set; }
        public DateTime deduct_start_date { get; set; }
        public string deduction_type { get; private set; }
        public decimal escrow_threshold { get; set; }
        public string frequency { get; set; }
        public decimal garnish_arrears { get; private set; }
        public decimal garnish_current { get; private set; }
        public decimal garnish_net_amt { get; private set; }
        public DateTime last_taken { get; private set; }
        public bool is_loan { get; private set; }
        public decimal loan_balance { get; private set; }
        public bool on_hold { get; set; }
        public decimal orig_loan_amt { get; set; }
        public string payee_id { get; set; }
        public decimal percentage { get; set; }
        public decimal rate { get; set; }
        public bool ready_to_pay_flag { get; set; }
        public decimal total_to_date { get; set; }
        public string tractor_id { get; set; }
        
    }
}
