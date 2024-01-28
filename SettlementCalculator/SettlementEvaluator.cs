using Microsoft.Azure.Cosmos.Linq;
using SettlementEvaluator;
using SettlementEvaluator.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SettlementEvaluator
{
    public enum CycleDays
    {
        W = 7
    }
    public interface ISettlementEvaluator
    {
        Task<bool> Evaluate(SettlementMessage settlement);
    }
    public class SettlementEvaluator : ISettlementEvaluator
    {
        IHttpClientFactory _httpClientFactory;
        IRecurringDeductionRepo _recurringDeductRepo;
        IPendingDeductionsRepo _pendingDeductRepo;
        ICarrierProfileRepo _carrierProfileRepo;
        IPayeeRepo _payeeRepo;
        IDeductionCodesRepo _deductionCodesRepo;
        Dictionary<string, DeductionCode> deductionCodes;
        CarrierProfile _carrier;
        public SettlementEvaluator() { }
        public SettlementEvaluator(IHttpClientFactory httpClientFactory, IRecurringDeductionRepo recurringDeductRepo, IPendingDeductionsRepo pendingDeductRepo, IPayeeRepo payeeRepo, ICarrierProfileRepo carrierProfileRepo, IDeductionCodesRepo deductionCodesRepo)
        {
            _httpClientFactory = httpClientFactory;
            _recurringDeductRepo = recurringDeductRepo;
            _pendingDeductRepo = pendingDeductRepo;
            _payeeRepo = payeeRepo;
            _carrierProfileRepo = carrierProfileRepo;
            _deductionCodesRepo = deductionCodesRepo;
        }

        public async Task<bool> Evaluate(SettlementMessage settlement)
        {
            try
            {
                _carrier = await _carrierProfileRepo.Get(settlement.CarrierId);

                var maxDeliveryDate = settlement.Settlements.Select(x => x.delivery_date).Max();
                
                var deductCodes = await _deductionCodesRepo.GetAll();
                deductionCodes = deductCodes.ToDictionary(keySelector: c => c.id);

                var payee = await _payeeRepo.Get(settlement.PayeeId);

                var pendingDeductions = _pendingDeductRepo.GetForSettlementCalc(payee.id);
                var recurringDeductions = _recurringDeductRepo.GetByPayeeId(payee.id);
                await Task.WhenAll(pendingDeductions, recurringDeductions);

                var recurringDeductionIds = pendingDeductions.Result.Select(x => x.recur_deduct_id).ToList();//get the recurring deduction ids so we can filter them out if they've already been processed into a pending deduct
                var netRecurringDeductions = recurringDeductions.Result.Where(d => !recurringDeductionIds.Contains(d.deduct_code_id)).ToList();

                var totalPay = settlement.TotalPay();
                var pendingDeductionAmount = await GetPendingDeductionsAmount(pendingDeductions.Result);
                var recurringDeductionAmount = await GetRecurringDeductionsAmount(netRecurringDeductions, maxDeliveryDate, totalPay);

                var availableAdvance = totalPay - pendingDeductionAmount - recurringDeductionAmount - payee.drspayee.taxable_owed;

                //if (availableAdvance > 0)
                //   await SendPaymentRailRequest();

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private async Task<bool> SendPaymentRailRequest()
        //{

        //}
        private async Task<decimal> GetPendingDeductionsAmount(List<PendingDeduction> deductions)
        {
            decimal deductionsAmount = 0;
            decimal earningsAmount = 0;
            return await Task.Run(() =>
            {
                deductions.ForEach(d =>
                {
                    DeductionCode deductCode;
                    if (!deductionCodes.TryGetValue(d.deduct_code_id, out deductCode))
                        throw new Exception("DeductionCode does not exist in master list");

                    if (deductCode.code_type == "D")
                        deductionsAmount += d.amount;
                    else
                        earningsAmount += d.amount;
                });
                return deductionsAmount - earningsAmount;
            });
        }
        private async Task<decimal> GetRecurringDeductionsAmount(List<RecurringDeduction> recurringDeductions, DateTime settlementDate, decimal grossSettlementAmount)
        {
            decimal deductionsAmount = 0;
            decimal earningsAmount = 0;
            List<RecurringDeduction> deductions = new List<RecurringDeduction>();
            return await Task.Run(() =>
            {
                recurringDeductions.ForEach(d =>
                    {
                        DeductionCode deductCode;
                        if (!deductionCodes.TryGetValue(d.deduct_code_id, out deductCode))
                            throw new Exception("DeductionCode does not exist in master list");

                        decimal amount = 0;
                        if (d.frequency == "C")
                            amount = CalcForCycle(d, settlementDate);

                        if (d.frequency == "P")
                            amount = d.amount * grossSettlementAmount;

                        if (deductCode.code_type == "D")
                            deductionsAmount += amount;
                        else
                            earningsAmount += amount;
                    });
                return deductionsAmount - earningsAmount;
            });
        }

        private decimal CalcForCycle(RecurringDeduction d, DateTime settlementDate)
        {

            CycleDays cycleDays;

            if (!Enum.TryParse<CycleDays>(d.cycle_id, true, out cycleDays))
                return 0;

            DateTime settlementStartDate = _carrier.PreviousSettlementDate(settlementDate);
            TimeSpan daysSinceLastTaken;
            int deductionInstanceCount;
            if (d.last_taken != null)
            {
                daysSinceLastTaken = settlementDate - (d.last_taken ?? DateTime.MinValue);
                deductionInstanceCount = daysSinceLastTaken.Days / Convert.ToInt32(cycleDays);
            }
            else
            {
                daysSinceLastTaken = new TimeSpan(Convert.ToInt32(cycleDays), 0, 0);
                deductionInstanceCount = 1;
            }



            return d.amount * deductionInstanceCount;
        }
    }
}
