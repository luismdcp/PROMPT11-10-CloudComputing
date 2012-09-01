using System;
using System.Collections.Generic;

namespace CloudNotes.Domain.Services.Models.WebAPI
{
    [Serializable]
    public class List
    {
        #region Properties

        private string partitionKey;
        private string rowKey;
        private string title;
        private IList<Link> links;

        public string PartitionKey
        {
            get { return partitionKey; }
            set { partitionKey = value; }
        }

        public string RowKey
        {
            get { return rowKey; }
            set { rowKey = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public IList<Link> Links
        {
            get { return links; }
            private set { links = value; }
        }

        #endregion Properties

        #region Constructors

        public List(string partitionKey, string rowKey, string title)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Title = title;
            Links = new List<Link>();
        }

        #endregion Constructors
    }
}