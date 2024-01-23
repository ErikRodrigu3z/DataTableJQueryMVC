namespace JQueryDataTablesMVC.Models
{    
    public class DataTableViewModel
    {        
        public string Url { get; set; }        
        public string EditUrl { get; set; } 
        public string NewUrl { get; set; } 
        public object Entity { get; set; }
        public bool StateSave { get; set; } 
        public int PageLenght { get; set; }

    }
}
