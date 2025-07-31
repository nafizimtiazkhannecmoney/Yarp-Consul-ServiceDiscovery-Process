
using System.Text.Json.Serialization;

namespace SARB_Reporting.Models.Regular
{
    public class RemittanceSarbData
    {
        [JsonPropertyName("dtt_mod")]
        public string DttMod { get; set; }

        [JsonPropertyName("tx_flow")]
        public string TxFlow { get; set; }

        [JsonPropertyName("tx_mtai")]
        public string TxMTAI { get; set; }

        [JsonPropertyName("is_active")]
        public int IsActive { get; set; }

        [JsonPropertyName("id_env_key")]
        public int IdEnvKey { get; set; }

        [JsonPropertyName("id_txn_key")]
        public int IdTxnKey { get; set; }

        [JsonPropertyName("tx_version")]
        public string TxVersion { get; set; }

        [JsonPropertyName("dt_last_run")]
        public string DtLastRun { get; set; }

        [JsonPropertyName("dt_txn_date")]
        public string DtTxnDate { get; set; }

        [JsonPropertyName("tx_ben_name")]
        public string TxBenName { get; set; }

        [JsonPropertyName("tx_ben_acc_no")]
        public string TxBenAccNo { get; set; }

        [JsonPropertyName("dec_ben_amount")]
        public decimal DecBenAmount { get; set; }

        [JsonPropertyName("tx_ben_country")]
        public string TxBenCountry { get; set; }

        [JsonPropertyName("tx_ben_surname")]
        public string TxBenSurname { get; set; }

        [JsonPropertyName("tx_branch_code")]
        public string TxBranchCode { get; set; }

        [JsonPropertyName("tx_branch_name")]
        public string TxBranchName { get; set; }

        [JsonPropertyName("tx_environment")]
        public string TxEnvironment { get; set; }

        [JsonPropertyName("tx_sequence_no")]
        public string TxSequenceNo { get; set; }

        [JsonPropertyName("dt_customer_dob")]
        public string DtCustomerDob { get; set; }

        [JsonPropertyName("id_customer_key")]
        public int IdCustomerKey { get; set; }

        [JsonPropertyName("id_user_mod_key")]
        public int IdUserModKey { get; set; }

        [JsonPropertyName("tx_bop_category")]
        public string TxBopCategory { get; set; }

        [JsonPropertyName("tx_reference_no")]
        public string TxReferenceNo { get; set; }

        [JsonPropertyName("id_fsm_state_key")]
        public int IdFsmStateKey { get; set; }

        [JsonPropertyName("tx_customer_city")]
        public string TxCustomerCity { get; set; }

        [JsonPropertyName("tx_customer_name")]
        public string TxCustomerName { get; set; }

        [JsonPropertyName("id_fsm_action_key")]
        public int IdFsmActionKey { get; set; }

        [JsonPropertyName("tx_customer_phone")]
        public string TxCustomerPhone { get; set; }

        [JsonPropertyName("tx_receiving_bank")]
        public string TxReceivingBank { get; set; }

        [JsonPropertyName("tx_ruling_section")]
        public string TxRulingSection { get; set; }

        [JsonPropertyName("id_beneficiary_key")]
        public int IdBeneficiaryKey { get; set; }

        [JsonPropertyName("tx_customer_acc_no")]
        public string TxCustomerAccNo { get; set; }

        [JsonPropertyName("tx_customer_gender")]
        public string TxCustomerGender { get; set; }

        [JsonPropertyName("tx_customer_suburb")]
        public string TxCustomerSuburb { get; set; }

        [JsonPropertyName("tx_orig_txn_ref_no")]
        public string TxOrigTxnRefNo { get; set; }

        [JsonPropertyName("dec_customer_amount")]
        public decimal DecCustomerAmount { get; set; }

        [JsonPropertyName("tx_customer_address")]
        public string TxCustomerAddress { get; set; }

        [JsonPropertyName("tx_customer_surname")]
        public string TxCustomerSurname { get; set; }

        [JsonPropertyName("tx_location_country")]
        public string TxLocationCountry { get; set; }

        [JsonPropertyName("tx_originating_bank")]
        public string TxOriginatingBank { get; set; }

        [JsonPropertyName("tx_ben_currency_code")]
        public string TxBenCurrencyCode { get; set; }

        [JsonPropertyName("tx_customer_province")]
        public string TxCustomerProvince { get; set; }

        [JsonPropertyName("tx_receiving_country")]
        public string TxReceivingCountry { get; set; }

        [JsonPropertyName("tx_ben_acc_identifier")]
        public string TxBenAccIdentifier { get; set; }

        [JsonPropertyName("tx_originating_country")]
        public string TxOriginatingCountry { get; set; }

        [JsonPropertyName("tx_reporting_qualifier")]
        public string TxReportingQualifier { get; set; }

        [JsonPropertyName("tx_customer_identity_no")]
        public string TxCustomerIdentityNo { get; set; }

        [JsonPropertyName("tx_customer_passport_no")]
        public string TxCustomerPassportNo { get; set; }

        [JsonPropertyName("tx_customer_postal_city")]
        public string TxCustomerPostalCity { get; set; }

        [JsonPropertyName("tx_customer_postal_code")]
        public string TxCustomerPostalCode { get; set; }

        [JsonPropertyName("tx_customer_contact_name")]
        public string TxCustomerContactName { get; set; }

        [JsonPropertyName("tx_customer_postal_suburb")]
        public string TxCustomerPostalSuburb { get; set; }

        [JsonPropertyName("tx_customer_acc_identifier")]
        public string TxCustomerAccIdentifier { get; set; }

        [JsonPropertyName("tx_customer_postal_address")]
        public string TxCustomerPostalAddress { get; set; }

        [JsonPropertyName("id_remittance_sarb_data_key")]
        public int IdRemittanceSarbDataKey { get; set; }

        [JsonPropertyName("id_remittance_sarb_data_ver")]
        public int IdRemittanceSarbDataVer { get; set; }

        [JsonPropertyName("tx_customer_contact_surname")]
        public string TxCustomerContactSurname { get; set; }

        [JsonPropertyName("tx_customer_postal_province")]
        public string TxCustomerPostalProvince { get; set; }

        [JsonPropertyName("tx_customer_passport_country")]
        public string TxCustomerPassportCountry { get; set; }
    }
}
