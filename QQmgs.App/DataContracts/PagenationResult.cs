using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Twitter.App.DataContracts
{
    public class PaginationResult<T>
    {
        [JsonProperty("items")]
        public IList<T> Items { get; set; }

        [JsonProperty("pagination_info")]
        public PaginationInfo PaginationInfo { get; set; }

        public PaginationResult(IEnumerable<T> items, long pageNo, long pageSize, long totalRecordsCount)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} of type {typeof(T)} is null");
            }

            this.Items = items.ToList();

            long pageCount = (totalRecordsCount + pageSize - 1) / pageSize;

            this.PaginationInfo = new PaginationInfo
            {
                PageNo = pageNo,
                PageSize = pageSize,
                PageCount = pageCount,
                TotalRecordsCount = totalRecordsCount
            };
        }

        private PaginationResult(IEnumerable<T> items, long pageNo, long pageSize, long pageCount, long? totalRecordsCount)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), $"{nameof(items)} of type {typeof(T)} is null");
            }

            this.Items = items.ToList();
            this.PaginationInfo = new PaginationInfo
            {
                PageNo = pageNo,
                PageSize = pageSize,
                PageCount = pageCount,
                TotalRecordsCount = totalRecordsCount
            };
        }

        [JsonConstructor]
        private PaginationResult()
        {
        }
    }

    public class PaginationInfo
    {
        [JsonProperty("page_no")]
        public long PageNo { get; set; }

        [JsonProperty("page_size")]
        public long PageSize { get; set; }

        [JsonProperty("page_count")]
        public long PageCount { get; set; }

        [JsonProperty("total_records_count")]
        public long? TotalRecordsCount { get; set; }
    }
}
