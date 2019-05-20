using System.Threading;
using Wacom.Ink.Model;

namespace WillDevicesSampleApp
{
	class InkDocumentDisplayItem
	{
		public InkDocumentDisplayItem(string errorMessage, string textDocument = "Document")
		{
			Id = Interlocked.Increment(ref _documentCounter);
			Document = null;
			ErrorMessage = errorMessage;
            TextDocument = textDocument;
        }

		public InkDocumentDisplayItem(InkDocument document, string textDocument = "Document", string textStrokes= "strokes")
		{
			Id = Interlocked.Increment(ref _documentCounter);
			Document = document;
			ErrorMessage = string.Empty;
            TextDocument = textDocument;
            TextStrokes = textStrokes;
		}

		public override string ToString()
		{
			if (Document != null)
			{
				int strokesCount = Document.GetStrokesCount();

				return $"{TextDocument} {Id} ({strokesCount} {TextStrokes})";
			}
			else
			{
				return $"{TextDocument} {Id} ({ErrorMessage})";
			}
		}

		public InkDocument Document { get; private set; }
		public int Id { get; private set; }
		public string ErrorMessage { get; private set; }
        public string TextStrokes { get; private set; }
        public string TextDocument { get; private set; }

        private static int _documentCounter = 0;
	}
}