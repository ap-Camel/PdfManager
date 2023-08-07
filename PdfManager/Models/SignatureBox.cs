namespace PdfManager.Models
{
    public class SignatureBox
    {
        /// <summary>
        /// x coordinate of where the signature box should be
        /// </summary>
        public int xAxis { get; set; }

        /// <summary>
        /// y coordinate of where the signature box should be
        /// </summary>
        public int yAxis { get; set; }

        /// <summary>
        /// the text that should be in the signature box
        /// </summary>
        public string text { get; set; } = string.Empty;
    }
}
