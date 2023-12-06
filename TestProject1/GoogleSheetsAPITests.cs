using ooadproject.Sheets;

namespace ProjectTests
{
    [TestClass]
    public class GoogleSheetsAPITests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetExamResults_InvalidLink_ReturnsEmptyResults()
        {
            // Arrange
            string link = "invalid link";

            // Act
            var results = SheetsFacade.GetExamResults(link);

            // Assert
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
            Assert.IsTrue(results.ContainsKey(19109));
            Assert.IsTrue(results.ContainsKey(19163));
            Assert.AreEqual(10, results[19163]);
            Assert.AreEqual(5, results[19109]);
        }

        [TestMethod]
        [ExpectedException (typeof(Exception))]
        public void GetExamResults_WrongFormat_ThrowsException()
        {
            // Arrange
            string link = "https://docs.google.com/spreadsheets/d/1xEbVjwSOi13Z_JS38Q9jjgRI2ePXr3xI4Ua10pixBS8/edit?usp=sharing";

            // Act
            var results = SheetsFacade.GetExamResults(link);

            // Assert
            
        }

        [TestMethod]
        public void ExtractSpreadsheetId_ValidLink_ReturnsSpreadsheetId()
        {
            // Arrange
            var manager = new SheetsFacade();
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


