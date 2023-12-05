using ooadproject.Sheets;

namespace ProjectTests
{
    [TestClass]
    public class GoogleSheetsAPITests
    {
        [TestMethod]
        public void GetExamResults_InvalidLink_ReturnsEmptyResults()
        {
            // Arrange
            string link = "invalid link";

            // Act
            var results = SheetsFacade.GetExamResults(link);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetExamResults_ValidLink_ReturnsResults()
        {
            // Arrange
            string link = "https://docs.google.com/spreadsheets/d/1vErXCyteFukR0smnSF8jN7nVhZznzaz2tNJo8pouWjU/edit#gid=0";

            // Act
            var results = SheetsFacade.GetExamResults(link);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.ContainsKey(1));
            Assert.IsTrue(results.ContainsKey(2));
            Assert.AreEqual(90.5, results[1]);
            Assert.AreEqual(85.2, results[2]);
        }

        [TestMethod]
        public void ExtractSpreadsheetId_ValidLink_ReturnsSpreadsheetId()
        {
            // Arrange
            string link = "https://docs.google.com/spreadsheets/d/1vErXCyteFukR0smnSF8jN7nVhZznzaz2tNJo8pouWjU/edit#gid=0";
            string expectedId = "1vErXCyteFukR0smnSF8jN7nVhZznzaz2tNJo8pouWjU";

            // Act
            string actualId = SheetsFacade.ExtractSpreadsheetId(link);

            // Assert
            Assert.AreEqual(expectedId, actualId);
        }

        [TestMethod]
        public void ExtractSpreadsheetId_InvalidLink_ReturnsEmptyString()
        {
            // Arrange
            string link = "invalid link";
            string expectedId = string.Empty;

            // Act
            string actualId = SheetsFacade.ExtractSpreadsheetId(link);

            // Assert
            Assert.AreEqual(expectedId, actualId);
        }
    }
}


