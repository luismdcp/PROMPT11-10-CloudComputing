using System;
using System.Collections.Generic;

namespace CloudNotes.Domain.Services.Models.WebAPI
{
    [Serializable]
    public class Task
    {
        #region Properties

        private string partitionKey;
        private string rowKey;
        private string title;
        private string content;
        private bool isClosed;
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

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public bool IsClosed
        {
            get { return isClosed; }
            set { isClosed = value; }
        }

        public IList<Link> Links
        {
            get { return links; }
            private set { links = value; }
        }

        #endregion Properties

        #region Constructors

        public Task(string partitionKey, string rowKey, string title, string content, bool isClosed)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Title = title;
            Content = content;
            IsClosed = isClosed;
            Links = new List<Link>();
        }

        #endregion Constructors
    }
}