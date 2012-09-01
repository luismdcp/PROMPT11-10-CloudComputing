using System;

namespace CloudNotes.Domain.Services.Models.WebAPI
{
    [Serializable]
    public class Link
    {
        #region Properties

        private string name;
        private string rel;
        private string href;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Rel
        {
            get { return rel; }
            set { rel = value; }
        }
       
        public string Href
        {
            get { return href; }
            set { href = value; }
        }

        #endregion Properties
    }
}