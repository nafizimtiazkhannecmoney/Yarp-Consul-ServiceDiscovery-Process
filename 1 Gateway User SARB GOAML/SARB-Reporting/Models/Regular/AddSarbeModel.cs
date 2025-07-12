namespace SARB_Reporting.Models.Regular
{
    public class AddSarbeModel
    {
        public int? is_active { get; set; }
        public int? id_env_key { get; set; }
        public int? id_fsm_action_key { get; set; }
        public int? id_fsm_state_key { get; set; }
        public int? id_user_mod_key { get; set; }

        public string? tx_txn_type { get; set; }        
        public string? tx_txn_ref_no { get; set; }      
        public string? tx_sarb_ref_no { get; set; }   
        public string? tx_file_name { get; set; }     
        public string? tx_response { get; set; }     
        public string? tx_retrieve_response { get; set; }     
        public string? tx_response_by_reference { get; set; }     
        public string? tx_warning_code { get; set; }     
        public string? tx_warning_details { get; set; }     
        public string? tx_sarb_err_desc { get; set; }     
    }
}
