namespace MedNidhiPlusBackEnd.API.Models;

public class DailyCollectionReportDto
{
    public DateTime Date { get; set; }

    public decimal TotalCollection { get; set; }

    public decimal CashCollection { get; set; }
    public decimal CardCollection { get; set; }
    public decimal UpiCollection { get; set; }
    public decimal OtherCollection { get; set; }
}


