namespace ooadproject.Sheets
{
    public class SheetsFacade
    {
        public SheetsFacade()
        {

        }
        public static string ExtractSpreadsheetId(string link)
        {
            string spreadsheetId = string.Empty;
            try
            {
                Uri uri = new Uri(link);
                string path = uri.AbsolutePath;
                string[] segments = path.Split('/');
                spreadsheetId = segments[segments.Length - 2];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting spreadsheet ID: " + ex.Message);
            }

            return spreadsheetId;
        }

        public static Dictionary<int, double> GetExamResults(string link)
        {
            return null;
        }
    }    

}
