using System;
public class AttendanceEntry
{
	public string WorkerID { get; set; }
	public string Client { get; set; }
	public int BU { get; set; } 
    public string Timeslot { get; set; } 
    public DateTime AttendanceDate { get; set; }
	public string TimeIn { get; set; }
	public string TimeOut { get; set; }
    public double TotalHours { get; set; }
 
   
}
