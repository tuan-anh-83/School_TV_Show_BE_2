namespace BOs.Models
{
    public class PayOSWebhookRequest
    {
        public string code { get; set; }      // Response Code (e.g., "00" for success)
        public string desc { get; set; }      // Description message
        public bool success { get; set; }     // ✅ This was missing!
        public PayOSWebhookData data { get; set; } // Nested data object
        public string signature { get; set; } // Signature for verification
    }


    public class PayOSWebhookData
    {
        public long orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string accountNumber { get; set; }
        public string reference { get; set; }
        public string transactionDateTime { get; set; }
        public string paymentLinkId { get; set; }
        public string currency { get; set; }
        public string code { get; set; }  // ✅ Thay status bằng code
        public string desc { get; set; }  // ✅ Thông tin giao dịch

        public string? counterAccountBankId { get; set; }
        public string? counterAccountBankName { get; set; }
        public string? counterAccountName { get; set; }
        public string? counterAccountNumber { get; set; }
        public string? virtualAccountName { get; set; }
        public string? virtualAccountNumber { get; set; }
    }


}
