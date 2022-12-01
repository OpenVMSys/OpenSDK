namespace OpenSDK
{
    public static class OmsConfiguration
    {
        public static string SiteName = "";
        public static string SiteUrl = "";
        public static string DatabaseUrl = "";
        //=================================================================
        // BELOW ARE PROGRAM SETTINGS, DO NOT EDIT UNLESS YOU ARE ASKED TO!
        //=================================================================
        //=================================================================
        // BELOW ARE PROGRAM SETTINGS, DO NOT EDIT UNLESS YOU ARE ASKED TO!
        //=================================================================
        //=====================
        // YOU HAVE BEEN WARNED
        //=====================
        public static readonly Version Version = new(2, 7, 142);
        //SecurityKeyModel Permission Level
        public enum SecurityPermission
        {
            Low=0,
            Mid=50,
            High=100,
            Top=999
        }
        //Flight Status
        public enum FlightStatus
        {
            Prepare,
            Push,
            Taxi,
            Takeoff,
            Climb,
            Cruise,
            Descend,
            Approach,
            Landed
        }
        //Flight Types
        public enum FlightTypes
        {
            Passenger,
            Cargo,
            
        }
        //Report Status
        public enum ReportStatus
        {
            Accepted,
            Declined,
            Pending
        }
    }
}