using System;
public class TimeslotMappingInfo
{
    public int RowNo { get; set; }
    public string TimeSlotCode { get; set; }
    
    public string TimeSlotDesc { get; set; }
    public string Description { get; set; }
	public class FieldName
	{
		public const string TimeSlotCode = "TimeSlotCode";
		public const string Description = "Description";
	}
}
