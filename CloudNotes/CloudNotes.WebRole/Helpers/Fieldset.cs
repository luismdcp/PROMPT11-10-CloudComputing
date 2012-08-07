using System;
using System.IO;
using System.Web.Mvc;

namespace CloudNotes.WebRole.Helpers
{
    public class Fieldset : IDisposable
    {
        #region Fields

        private readonly TextWriter _writer;

        #endregion Fields

        #region Constructor

        public Fieldset(ViewContext viewContext)
        {
            _writer = viewContext.Writer;
        }

        #endregion Constructor

        #region Public methods

        public void Dispose()
        {
            _writer.WriteLine("</fieldset>");
        }

        #endregion Public methods
    }
}