namespace KDMSServer.Model
{
    public class PdbListModel
    {
        public int PdbId { get; set; }
        public string PdbName { get; set; }
        public string PdbMd5 { get; set; }
        public bool IsModify { get; set; }
    }
}